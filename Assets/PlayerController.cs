using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    private Vector2 moveInput;
    [SerializeField] float moveSpeed = 1f;

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

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (!IsGrounded()) TogglePlayerGravity();
    }

    private void FixedUpdate()
    {

    }

    void MovePlayer()
    {
        // Apply movement based on input
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f; // Ensure the camera is not pointing up or down
        cameraForward.Normalize();

        // Transform the movement vector to align with the camera's direction
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0f; // Disable any vertical movement

        gameObject.transform.Translate(movement * moveSpeed * Time.deltaTime);
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
        float maxDistance = 1.0f;

        RaycastHit hit;
        if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance)) return true;
        else return false;
    }
}
