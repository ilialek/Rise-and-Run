using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The PlayerScript class handles player movement, jumping, and wall-running mechanics.
// It includes ground and wall checks, force applications, and interaction with level transitions.
public class PlayerScript : MonoBehaviour
{
    // Private variables for components
    private Camera mainCamera; // Main camera reference
    private Rigidbody rb; // Rigidbody component reference
    private AudioManager audioManager; // AudioManager reference

    [Header("Ground movement and jump settings")]
    [SerializeField] private float speed = 12f; // Movement speed
    [SerializeField] private float groundDrag; // Drag applied when on the ground
    [SerializeField] private float airFriction; // Friction applied when in the air
    [SerializeField] private float jumpForce; // Force applied when jumping
    [SerializeField] private LayerMask whatIsGround; // Layer mask to identify ground
    [SerializeField] private bool isGrounded; // Boolean to check if player is grounded

    [Header("Wall run settings")]
    [SerializeField] private LayerMask whatIsWall; // Layer mask to identify walls
    [SerializeField] private float wallRunForce; // Force applied during wall run
    [SerializeField] private float wallJumpForce; // Force applied when jumping off a wall
    [SerializeField] private float wallJumpSideForce; // Side force applied when jumping off a wall
    [SerializeField] private float wallCheckDistance; // Distance for wall checks
    [SerializeField] private float minJumpHeight; // Minimum height required for jumping
    [SerializeField] private float exitWallTime; // Time to wait after exiting a wall run

    private RaycastHit leftWallHit; // Raycast hit for left wall check
    private RaycastHit rightWallHit; // Raycast hit for right wall check

    private bool wallLeft; // Boolean to check if there's a wall on the left
    private bool wallRight; // Boolean to check if there's a wall on the right
    private bool isWallRunning; // Boolean to check if the player is wall-running

    private bool exitingWall = false; // Boolean to check if the player is exiting a wall run
    private float exitWallTimer; // Timer for exiting wall run

    [Header("Level loader")]
    [SerializeField] private LevelLoader levelLoader; // Reference to the LevelLoader script

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component and freeze its rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Get the main camera reference
        mainCamera = Camera.main;

        // Find the LevelLoader object in the scene and get its LevelLoader component
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for walls
        CheckForWall();

        // Handle ground movement, jumping, and speed control when not wall-running
        if (!isWallRunning)
        {
            JumpAndDrag();
            SpeedControl();
        }

        // Handle wall jumping when space key is pressed during wall run
        if (Input.GetKeyDown(KeyCode.Space) && isWallRunning)
        {
            Debug.Log("Jump oof the wall");
            WallJump();
        }

        // Handle wall exit timer and revert FOV when exiting a wall run
        if (exitingWall)
        {
            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }

            RevertFOV();
        }
    }

    // FixedUpdate is called at a fixed interval
    void FixedUpdate()
    {
        // Handle ground movement when not wall-running
        if (!isWallRunning)
        {
            GroundMovement();
        }

        // Handle wall running when conditions are met
        if ((wallLeft || wallRight) && Input.GetAxisRaw("Vertical") != 0 && AboveGround() && !exitingWall)
        {
            isWallRunning = true;
            WallRunning();
            ChangeFOV();
        }
        else
        {
            isWallRunning = false;
            StopWallRun();
        }
    }

    // Method to handle wall running mechanics
    void WallRunning()
    {
        rb.useGravity = false; // Disable gravity

        // Zero out the vertical velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Determine the wall normal and forward direction
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // Ensure the correct wall forward direction
        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // Apply wall run force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    // Method to stop wall running
    void StopWallRun()
    {
        rb.useGravity = true; // Enable gravity
    }

    // Method to handle wall jumping mechanics
    void WallJump()
    {
        exitingWall = true; // Set exitingWall to true
        exitWallTimer = exitWallTime; // Start exit wall timer

        // Determine the wall normal and force to apply for wall jump
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

        // Zero out the vertical velocity and apply jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    // Method called when the player collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // Apply an upward force when colliding with a jump platform
        if (collision.gameObject.layer == LayerMask.NameToLayer("JumpPlatform"))
        {
            rb.AddForce(transform.up * 65, ForceMode.Impulse);
        }
    }

    // Method called when the player enters a trigger collider
    void OnTriggerEnter(Collider other)
    {
        // Load the next level when colliding with the finish trigger
        if (other.gameObject.layer == LayerMask.NameToLayer("Finish"))
        {
            levelLoader.LoadNextLevel();
        }
    }

    // Method to handle ground movement
    void GroundMovement()
    {
        float x = Input.GetAxisRaw("Horizontal"); // Get horizontal input
        float z = Input.GetAxisRaw("Vertical"); // Get vertical input

        Vector3 move = transform.right * x + transform.forward * z; // Calculate movement vector

        // Apply movement force based on ground or air state
        if (isGrounded)
        {
            rb.AddForce(move.normalized * speed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(move.normalized * speed * 10f * airFriction, ForceMode.Force);
        }
    }

    // Method to change the field of view (FOV) during wall running
    void ChangeFOV()
    {
        if (mainCamera.fieldOfView < 65)
        {
            mainCamera.fieldOfView++;
        }
        else
        {
            mainCamera.fieldOfView = 65;
        }
    }

    // Method to revert the field of view (FOV) after wall running
    void RevertFOV()
    {
        if (mainCamera.fieldOfView > 55)
        {
            mainCamera.fieldOfView--;
        }
        else
        {
            mainCamera.fieldOfView = 55;
        }
    }

    // Method to check for walls on the left and right sides
    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    // Method to control player speed
    void SpeedControl()
    {
        Vector3 XZVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Get horizontal velocity

        // Clamp the horizontal speed to the maximum speed
        if (XZVelocity.magnitude > speed)
        {
            Vector3 normalizedVelocity = XZVelocity.normalized * speed;
            rb.velocity = new Vector3(normalizedVelocity.x, rb.velocity.y, normalizedVelocity.z);
        }
    }

    // Method to handle jumping and drag based on ground state
    void JumpAndDrag()
    {
        // Check if the player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f, whatIsGround);

        // Apply drag based on ground state
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // Apply jump force when space key is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Method to check if the player is above the minimum jump height
    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }
}
