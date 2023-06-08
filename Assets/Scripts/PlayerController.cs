using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    private Vector2 moveInput;
    private bool dashInput;

    [SerializeField] private GameObject body;
    [SerializeField] private Animator animator;
    private int animationAttackHash;

    [SerializeField] GameObject weaponHand;

    [SerializeField] float moveSpeed = 1f;

    [SerializeField] private InputActionReference move, dash, attack;

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
    private void OnEnable()
    {
        dash.action.performed += PerformDash;
        attack.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        dash.action.performed -= PerformDash;
        attack.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext context)
    {
        SwingSword();
    }

    bool isDashing;
    private void PerformDash(InputAction.CallbackContext context)
    {
        isDashing = true;
    }


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
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animationAttackHash = Animator.StringToHash("SwingSword1");
    }

    // Update is called once per frame
    void Update()
    {
        CheckAnimationStates();
        MovePlayer();
        if (!IsGrounded() && !isDashing) TogglePlayerGravity();
        if (canHit)
        {
            CheckForHitable();
        }
    }

    Quaternion targetRotation;

    float dashLength = 0.2f;
    float initDashLength = 0.2f;
    float dashRecharge = 0.5f;
    float initdashRecharge = 0.5f;
    Vector3 lastMovementDirection = Vector3.zero; // Variable to store the last movement direction

    void MovePlayer()
    {
        moveInput = move.action.ReadValue<Vector2>();
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f; // Ensure the camera is not pointing up or down
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

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
            lastMovementDirection = movement.normalized;
        }

        if (isDashing && dashRecharge <= 0)
        {
            dashRecharge = 0;
            dashLength -= Time.deltaTime;
            if (!CheckWallCollision(lastMovementDirection.normalized))
            {
                // Movement when dashing
                gameObject.transform.Translate(lastMovementDirection * moveSpeed * 4f * Time.deltaTime);
            }
            else if (CheckForSlope(lastMovementDirection.normalized) || CheckForStep(lastMovementDirection.normalized))
            {
                // Dash up steps and slopes
                lastMovementDirection += Vector3.up;
                gameObject.transform.Translate(lastMovementDirection * moveSpeed * 4f * Time.deltaTime);
            }
            else
            {
                // If hit wall
                isDashing = false;
                dashLength = initDashLength;
                dashRecharge = initdashRecharge;
                lastMovementDirection = Vector3.zero;
            }

            if (dashLength <= 0)
            {
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
    }

    void TogglePlayerGravity()
    {
        gameObject.transform.Translate(-Vector3.up * 9.81f * Time.deltaTime);
    }

    bool canHit;
    [SerializeField] float weaponRayLength;
    [SerializeField] LayerMask hitableMasks;
    void SwingSword()
    {
        canHit = true;     
        animator.SetBool("Attack", true);
    }

    void CheckForHitable()
    {
        GameObject currentWeaponRaycast = weaponHand.transform.GetChild(0).GetChild(0).GetChild(0).gameObject; //Hand/HandPivot/Weapon/WeaponRaycast
        Debug.DrawRay(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, Color.red, weaponRayLength);
        if (Physics.Raycast(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, out RaycastHit hit, weaponRayLength, hitableMasks) && canHit)
        {
            ObjectExample targetObject = hit.collider.gameObject.GetComponent<ObjectExample>();
            this.gameObject.GetComponent<ObjectExample>().attackOtherObject = targetObject;
            this.gameObject.GetComponent<ObjectExample>().DoAttack();
        }
    }

    void CheckAnimationStates()
    {
        // Check if the desired animation state has finished playing
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("SwingSword1"))
        {
            // Animation has finished playing
            animator.SetBool("Attack", false);
            canHit = false;
        }
    }

    [SerializeField] private LayerMask terrainMasks;
    private bool CheckWallCollision(Vector3 movementDirection)
    {
        // Perform a raycast in the movement direction to check for wall collision
        RaycastHit hit;
        if (Physics.Raycast(transform.position, movementDirection, out hit, 0.5f, terrainMasks))
        {
            // Wall collision detected
            return true;
        }

        return false;
    }

    [SerializeField] private Transform feetTransform;
    [SerializeField] private float maxSlopeAngle = 45;
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

        if (Physics.Raycast(feetTransform.position, movementDirection, out RaycastHit hit, 0.5f, terrainMasks))
        {
                Debug.Log("OnSlope");
                return true;
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
