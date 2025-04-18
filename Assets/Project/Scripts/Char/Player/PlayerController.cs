using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DunGen;

public class PlayerController : MonoBehaviour, IKillable
{
    // Singleton
    private static PlayerController instance;
    public static PlayerController Instance { get => instance; }

    [SerializeField] private bool spawnAfterGen = false;

    #region Stats
    private int healthCap = 9;
    private int health = 9;
    #endregion

    #region Physics/Components
    [Header("Components")]
    [SerializeField] private GameObject body;
    private Animator animator;
    [SerializeField] private GameObject weaponHand;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference moveX;
    [SerializeField] private InputActionReference moveY;
    [SerializeField] private InputActionReference dodge;
    [SerializeField] private InputActionReference attack;
    private float moveInputX;
    private float moveInputY;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;

    [SerializeField] private float dodgeLength = 0.2f;
    [SerializeField] private float dodgeRecharge = 0.5f;
    private float initDodgeLength;
    private float initDodgeRecharge;

    [SerializeField] private float swingMomentumLength = 0.1f;
    [SerializeField] private float swingStrength = 1f;
    private float initswingMomentumLength;

    [Header("Terrain Settings")]
    [SerializeField] private LayerMask terrainMasks;
    [SerializeField] private Transform feetTransform;
    [SerializeField] private float maxSlopeAngle = 45;

    [Header("Weapon Settings")]
    [SerializeField] private float timeForCombo = 0.3f;
    private float initTimeForCombo;
    private int comboIndex = 0;

    [SerializeField] private float weaponRayLength;
    [SerializeField] private LayerMask hitableMasks;

    [Header("Animation Settings")]
    [SerializeField] private AnimationClip[] attackingClips;
    [SerializeField] private float[] comboFinishTimes;
    [SerializeField] private string currentAttackAnimatorClipName;
    [SerializeField] private float currentComboFinishTime;
    [SerializeField] private string currentAnimatorBoolName;
    [SerializeField] private AnimationEvent evt;

    // Movement Varibles
    private Quaternion targetRotation;
    private Vector3 lastMovementDir = Vector3.zero; 

    [SerializeField] private bool isDodging;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isSwinging;
    [SerializeField] private bool inAttackingStance;
    #endregion

    // Unity Methods
    private void Awake()
    {
        instance = this;
        this.animator = GetComponent<Animator>();
    }

    private void OnEnable() 
    {
        this.moveX.action.Enable();
        this.moveY.action.Enable();
        this.attack.action.Enable();
        this.dodge.action.Enable();

        this.attack.action.started += (ctx) =>
        {
            SwingWeaponAction("Attack");
        };

        this.dodge.action.started += (ctx) =>
        {
            DodgeAction();
        };
    }

    private void OnDisable()
    {
        this.moveX.action.Disable();
        this.moveY.action.Disable();
        this.attack.action.Disable();
        this.dodge.action.Disable();

        this.attack.action.started += (ctx) =>
        {
            SwingWeaponAction("Attack");
        };

        this.dodge.action.started -= (ctx) =>
        {
            DodgeAction();
        };

        DungeonGenerator.OnAnyDungeonGenerationStatusChanged -= OnGenerationStatusChanged;
    }

    void Start()
    {
        initDodgeLength = dodgeLength;
        initDodgeRecharge = dodgeRecharge;
        initswingMomentumLength = swingMomentumLength;
        initTimeForCombo = timeForCombo;

        currentAttackAnimatorClipName = attackingClips[0].name;
        currentComboFinishTime = comboFinishTimes[0];
        //currentAnimatorBoolName = "Attack";

        if (spawnAfterGen) DungeonGenerator.OnAnyDungeonGenerationStatusChanged += OnGenerationStatusChanged;
        else PlayerSpawn.SpawnPlayer();
    }

    void Update()
    {
        if (!IsAlive()) return;
        MovePlayer();
        if (!IsGrounded()) TogglePlayerGravity();
        CheckForHitable();
    }

    void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
    {
        if (status == GenerationStatus.Complete) PlayerSpawn.SpawnPlayer();
    }

    // Methods

    #region Actions
    void SwingWeaponAction(string boolName)
    {
        if (!IsAlive()) return;
        if (!isDodging && timeForCombo == initTimeForCombo)
        {
            if (comboIndex == 0)
            {
                isAttacking = true;
                //comboIndex = 1;
            }
            inAttackingStance = true;
            animator.SetBool(boolName, true);
            currentAnimatorBoolName = boolName;
        }
    }

    void DodgeAction()
    {
        if (!IsAlive()) return;
        if (!isAttacking)
        {
            isDodging = true;
            // isAttacking = false;
        }
    }
    #endregion

    #region Movement
    private void MovePlayer()
    {
        if (!isDodging) moveInputX = moveX.action.ReadValue<float>();
        if (!isDodging) moveInputY = moveY.action.ReadValue<float>();

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f; // Ensure the camera is not pointing up or down
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Vector3 movement = cameraRight * moveInput.x + cameraForward * moveInput.y;
        Vector3 movement = cameraRight * moveInputX + cameraForward * moveInputY;
        movement.y = 0f; // Disable any vertical movement
        movement.Normalize(); // Normalize the movement vector to ensure its magnitude does not exceed 1

        dodgeRecharge -= Time.deltaTime;

        if (dodgeRecharge < 0f) dodgeRecharge = 0f;

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
            lastMovementDir = movement.normalized;
        }

        if (isDodging && dodgeRecharge <= 0)
        {
            animator.SetBool("Roll", true);
            dodgeRecharge = 0;
            if (CheckForSlope(lastMovementDir.normalized) || CheckForStep(lastMovementDir.normalized))
            {
                // Dash up steps and slopes
                lastMovementDir += Vector3.up * 0.5f;
                // gameObject.transform.Translate(lastMovementDir * moveSpeed * 2f * Time.deltaTime);
            }
            if (!CheckWallCollision(lastMovementDir.normalized))
            {
                // Movement when dashing
                gameObject.transform.Translate(lastMovementDir * moveSpeed * 1.5f * Time.deltaTime);
            }
            else
            {
                // TODO add bonk

                // If hit wall
                //animator.SetBool("Roll", false);
                //isDodging = false;
                //dodgeLength = initDodgeLength;
                //dodgeRecharge = initDodgeRecharge;
                //lastMovementDir = Vector3.zero;
            }

            if (IsAnimationClipFinished("PlayerRoll", 1.0f))
            {
                animator.SetBool("Roll", false);
                isDodging = false;
                dodgeLength = initDodgeLength;
                dodgeRecharge = initDodgeRecharge;
            }
        }
        else
        {
            // Move player
            if (inAttackingStance) { } //gameObject.transform.Translate(movement * moveSpeed * 0.1f * Time.deltaTime);
            else gameObject.transform.Translate(movement * moveSpeed * Time.deltaTime);

            if (movement.x > 0 || movement.x < 0 || movement.z > 0 || movement.z < 0) animator.SetFloat("Movement", 1);
            else animator.SetFloat("Movement", 0);

            if (isSwinging)
            {
                if (CheckForSlope(lastMovementDir.normalized) || CheckForStep(lastMovementDir.normalized))
                {
                    lastMovementDir += Vector3.up;
                }
                if (!CheckWallCollision(lastMovementDir.normalized)) gameObject.transform.Translate(lastMovementDir * moveSpeed * swingStrength * Time.deltaTime);

                swingMomentumLength -= Time.deltaTime;
                if (swingMomentumLength <= 0) 
                {
                    swingMomentumLength = initswingMomentumLength;
                    isSwinging = false;
                }
            }
        }

        if (movement.magnitude > 0f)
        {
            targetRotation = Quaternion.LookRotation(movement);
        }

        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targetRotation, 8f * Time.deltaTime);
    }

    public void MoveOnSwing() 
    {
        isSwinging = true;
    }
    #endregion

    #region Physics
    void CheckForHitable()
    {
        //if (!isAttacking)
        //{
        //    isAttacking = false;
        //    animator.SetBool(currentAnimatorBoolName, false);
        //    currentAttackAnimatorClipName = attackingClips[0].name;

        //    timeForCombo = initTimeForCombo;
        //    return;
        //}

        GameObject currentWeaponRaycast = weaponHand.transform.GetChild(0).GetChild(0).GetChild(0).gameObject; //Hand/HandPivot/Weapon/WeaponRaycast
        Debug.DrawRay(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, Color.red, weaponRayLength);
        if (Physics.Raycast(currentWeaponRaycast.transform.position, currentWeaponRaycast.gameObject.transform.forward, out RaycastHit hit, weaponRayLength, hitableMasks))
        {
            ObjectSystem targetObject = hit.collider.gameObject.GetComponent<ObjectSystem>();
            this.gameObject.GetComponent<ObjectSystem>().attackOtherObject = targetObject;
            this.gameObject.GetComponent<ObjectSystem>().DoAttack();
        }

        if (IsAnimationClipFinished(currentAttackAnimatorClipName, currentComboFinishTime) && isAttacking)
        {
            timeForCombo -= Time.deltaTime;
            if (timeForCombo <= 0) 
            {
                comboIndex = 0;
                inAttackingStance = false;
                isAttacking = false;
                animator.SetBool(currentAnimatorBoolName, false);
                timeForCombo = initTimeForCombo;
                currentAttackAnimatorClipName = attackingClips[0].name;
                currentComboFinishTime = comboFinishTimes[0];
            }
            else if (attack.action.IsPressed() && comboIndex < attackingClips.Length - 1)
            {
                comboIndex++;
                Debug.Log("comboIndex" + comboIndex);
                animator.SetTrigger("Combo");
                // inAttackingStance = true;
                timeForCombo = initTimeForCombo;
                currentAttackAnimatorClipName = attackingClips[comboIndex].name;
                currentComboFinishTime = comboFinishTimes[comboIndex];
            }
        }
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
                // Debug.Log("OnSlope");
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
                //Debug.Log("OnStep");
                return true;
            }
        }

        return false;
    }

    void TogglePlayerGravity()
    {
        gameObject.transform.Translate(-Vector3.up * 9.81f * Time.deltaTime);
    }

    bool IsGrounded()
    {
        // Manually setting SphereCast
        Vector3 origin = transform.position;
        Vector3 direction = -Vector3.up;
        float radius = 0.5f;
        float maxDistance = 0.5f;

        RaycastHit hit;
        if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance, terrainMasks)) return true;
        else return false;
    }
    #endregion

    #region Animations
    bool IsAnimationClipFinished(string clipName, float finishNumber) 
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsName(clipName) && stateInfo.normalizedTime >= finishNumber;
    }
    #endregion

    //IKillable ------------------------------------------------------------------------

    public void Heal(int amount) 
    {
        health = health + amount;
        if (health > healthCap) health = healthCap;
    }

    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health < 0) health = 0;
    }

    public bool IsAlive()
    {
        if (health == 0) return false;
        else return true;
    }
}
