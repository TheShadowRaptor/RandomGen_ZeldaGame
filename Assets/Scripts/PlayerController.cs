using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Singleton
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<PlayerController>();
                    singletonObject.name = typeof(PlayerController).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    [Header("Components")]
    [SerializeField] private GameObject body;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponHand;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private InputActionReference attack;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashLength = 0.2f;
    [SerializeField] private float dashRecharge = 0.5f;
    private float initDashLength;
    private float initdashRecharge;

    [Header("Weapon Settings")]
    [SerializeField] private float weaponRayLength;
    [SerializeField] private LayerMask hitableMasks;

    [Header("Slope Settings")]
    [SerializeField] private LayerMask terrainMasks;
    [SerializeField] private Transform feetTransform;
    [SerializeField] private float maxSlopeAngle = 45;

    // Movement Varibles
    private Quaternion targetRotation;
    private Vector3 lastMovementDirectionSlope = Vector3.zero; 

    private bool isDashing;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        this.dash.action.performed += (ctx) => this.isDashing = true;
        this.attack.action.performed += (ctx) => this.SwingSword();
    }

    private void OnDisable()
    {
        this.dash.action.performed -= (ctx) => this.isDashing = true;
        this.attack.action.performed -= (ctx) => this.SwingSword();
    }

    // Start is called before the first frame update
    void Start()
    {
        initDashLength = dashLength;
        initdashRecharge = dashRecharge;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move.action.phase != InputActionPhase.Waiting) MovePlayer();
        if (!IsGrounded() && !isDashing) TogglePlayerGravity();
    }

    private void MovePlayer()
    {
        moveInput = move.action.ReadValue<Vector2>();

        Debug.Log("MoveInput: " + moveInput); // Log move input values

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f; // Ensure the camera is not pointing up or down
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Vector3 movement = cameraRight * moveInput.x + cameraForward * moveInput.y;
        Vector3 movement = cameraRight * moveInput.x + cameraForward * moveInput.y;
        movement.y = 0f; // Disable any vertical movement
        movement.Normalize(); // Normalize the movement vector to ensure its magnitude does not exceed 1

        dashRecharge -= Time.deltaTime;

        // Check for wall collision
        if (CheckWallCollision(movement.normalized))
        {
            // If wall collision detected, stop movement
            movement = Vector3.zero;
        }

        // Check for slope and step detection
        else if (CheckForSlope(movement.normalized) || CheckForStep(movement.normalized))
        {
            // If on a slope or step, move up
            gameObject.transform.Translate(Vector3.up * moveSpeed * 2 * Time.deltaTime);
        }

        // Store the last movement direction when not dashing
        if (movement.magnitude > 0f)
        {
            lastMovementDirectionSlope = movement.normalized;
        }

        if (isDashing && dashRecharge <= 0)
        {
            animator.SetBool("Roll", true);
            dashRecharge = 0;
            dashLength -= Time.deltaTime;
            if (CheckForSlope(lastMovementDirectionSlope.normalized) || CheckForStep(lastMovementDirectionSlope.normalized))
            {
                // Dash up steps and slopes
                lastMovementDirectionSlope += Vector3.up;
                gameObject.transform.Translate(lastMovementDirectionSlope * moveSpeed * 2f * Time.deltaTime);
            }
            if (!CheckWallCollision(lastMovementDirectionSlope.normalized))
            {
                // Movement when dashing
                gameObject.transform.Translate(lastMovementDirectionSlope * moveSpeed * 2f * Time.deltaTime);
            }
            else
            {
                // If hit wall
                animator.SetBool("Roll", false);
                isDashing = false;
                dashLength = initDashLength;
                dashRecharge = initdashRecharge;
                lastMovementDirectionSlope = Vector3.zero;
            }

            if (dashLength <= 0)
            {
                animator.SetBool("Roll", false);
                isDashing = false;
                dashLength = initDashLength;
                dashRecharge = initdashRecharge;
            }
        }
        else
        {
            // Move player
            gameObject.transform.Translate(movement * moveSpeed * Time.deltaTime);
               
        }

        if (movement.magnitude > 0f)
        {
            targetRotation = Quaternion.LookRotation(movement);
        }

        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targetRotation, 8f * Time.deltaTime);
        Debug.Log(moveSpeed);
    }

    void TogglePlayerGravity()
    {
        gameObject.transform.Translate(-Vector3.up * 9.81f * Time.deltaTime);
    }

    void SwingSword()
    {
        animator.SetBool("Attack", true);
        StartCoroutine(CheckForHitable());   
    }

    IEnumerator CheckForHitable()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) 
        {
            GameObject currentWeaponRaycast = weaponHand.transform.GetChild(0).GetChild(0).GetChild(0).gameObject; //Hand/HandPivot/Weapon/WeaponRaycast
            Debug.DrawRay(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, Color.red, weaponRayLength);
            if (Physics.Raycast(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, out RaycastHit hit, weaponRayLength, hitableMasks))
            {
                ObjectSystem targetObject = hit.collider.gameObject.GetComponent<ObjectSystem>();
                this.gameObject.GetComponent<ObjectSystem>().attackOtherObject = targetObject;
                this.gameObject.GetComponent<ObjectSystem>().DoAttack();
            }
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool("Attack", false);
        yield break;
    }

    private bool CheckWallCollision(Vector3 movementDirection)
    {
        Vector3 playerTorso = transform.position;
        Vector3 playerLegs = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        // Perform a raycast in the movement direction to check for wall collision
        RaycastHit hit;
        if (Physics.Raycast(playerTorso, movementDirection, out hit, 0.3f, terrainMasks))
        {
            // Wall collision detected
            return true;
        }
        else if (Physics.Raycast(playerLegs, movementDirection, out hit, 0.5f, terrainMasks))
        {
            // Wall collision detected
            return true;
        }
        Debug.DrawRay(playerLegs, movementDirection, Color.green, 0.5f);

        return false;
    }

    bool CheckForSlope(Vector3 movementDirection)
    {
        float slopeRayLength = 1.0f; // Adjust the length based on the maximum slope height you want to handle

        Vector3 slopeRayOrigin = feetTransform.position;
        slopeRayOrigin += Vector3.up * 0.1f; // Adjust the offset to avoid casting the ray from inside the collider

        if (Physics.Raycast(slopeRayOrigin, movementDirection, out RaycastHit hit, slopeRayLength, terrainMasks))
        {
            // Calculate the slope angle
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Check if the slope angle is within the acceptable range
            if (slopeAngle > 0f && slopeAngle <= maxSlopeAngle)
            {
                Debug.Log("OnSlope");
                return true;
            }
        }

        return false;
    }

    bool CheckForStep(Vector3 movementDirection)
    {
        Vector3 stepMaxHeight = new Vector3(feetTransform.position.x, feetTransform.position.y + 0.5f, feetTransform.position.z);
        if (Physics.Raycast(feetTransform.position, movementDirection, out RaycastHit feetHit, 0.5f, terrainMasks))
        {
            if (!Physics.Raycast(stepMaxHeight, movementDirection, out RaycastHit heightHit, 0.5f, terrainMasks))
            {
                Debug.Log("OnStep");
                return true;
            }
        }

        return false;
    }

    bool IsGrounded()
    {
        // Manually setting SphereCast
        Vector3 origin = transform.position;
        Vector3 direction = -Vector3.up;
        float radius = 0.5f;
        float maxDistance = 0.5f;

        RaycastHit hit;
        if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance)) return true;
        else return false;
    }

}
