using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hierarchal State Machine to be used by the Player
/// <para>Contains information on current state of the player movement</para>
/// </summary>
public class PlayerStateMachine
{
    #region getters/setters

    // Movement variables

    /// <summary>
    /// The curve to which ramp up the needed acceleration to account for the instantenous change in speed
    /// </summary>
    public AnimationCurve AccelerationMultiplier;

    /// <summary>
    /// The current speed of the player
    /// </summary>
    public float currentSpeed { get; set; } = 0f;

    /// <summary>
    /// Speed of the player to move at, Deprecated since acceleration will be used but this is currently used in determining roll distance
    /// </summary>
    public float speed { get; set; } = 20.0f;

    /// <summary>
    /// A multiplier to go on top of the movement functions in the future (Faster/slower from skills and or status effects)
    /// </summary>
    public float speedMultiplier { get; set; } = 1f;

    /// <summary>
    /// The flat speed multiplier when player is sprinting
    /// </summary>
    public float sprintModifier { get; set; } = 1.5f; 

    /// <summary>
    /// The acceleration the player will have
    /// </summary>
    public float acceleration { get; set; } = 100f;

    /// <summary>
    ///  The maximum acceleration the player CAN have
    /// </summary>
    public float maxAcceleration { get; set; } = 80f;

    /// <summary>
    ///  The amount of force the impulse will exert on the player when jumping
    /// </summary>
    public float jumpForce { get; set; } = 20.0f;

    /// <summary>
    ///  The maximum number of jumps the player has currently
    /// </summary>
    public float numJumpsMax { get; set; }

    /// <summary>
    ///  The number of jumps the player has taken out of their maximum
    /// </summary>
    public float numJumpsTaken { get; set; }

    /// <summary>
    /// The decceleration of input damping (NOT FOR MOVEMENT)
    /// </summary>
    private float decceleration = 8f;

    /// <summary>
    /// The boundary between the player and collision detection services
    /// </summary>
    public float skinWidth = 0.05f;


    /// <summary>
    /// Reference to the players RigidBody component
    /// </summary>
    public Rigidbody playerRb;

    /// <summary>
    /// Reference to the players CapsuleCollider component
    /// </summary>
    public CapsuleCollider playerCap { get; set; }

    /// <summary>
    /// Reference to the camera system
    /// </summary>
    public CameraController camera { get; set; }

    /// <summary>
    /// Reference to the players Animator component
    /// </summary>
    public Animator playerAnim { get; set; }

    /// <summary>
    /// The current amount of horizontal input: a, d
    /// </summary>
    public float horizontalInput { get; set; }

    /// <summary>
    /// The current amount of vertical input: w, s
    /// </summary>
    public float verticalInput { get; set; }

    #region Movement booleans
    // -----------------------
    public bool onGround { get; set; } = false;
    public bool canJump { get; set; } = true;
    public bool willJump { get; set; } = false;

    public bool isJumping { get; set; } = false;
    public bool isFalling { get; set; } = false;
    public bool isWalking { get; set; } = false;
    public bool isSprinting { get; set; } = false;
    public bool isCrouched { get; set; } = false;
    public bool isSliding { get; set; } = false;
    public bool checkGround { get; set; } = true;
    public bool isLockedOn { get; set; } = false;
    public bool isRolling { get; set; } = false;
    // -----------------------
    #endregion

    #region Combat booleans
    // -----------------------
    public bool isAttacking { get; set; } = false;
    public bool hasWeapon { get; set; } = false;
    public bool hasShield { get; set; } = false;
    public bool isEquipped { get; set; } = true;
    // -----------------------
    #endregion


    public Vector3 gravityVector { get; set; } = new Vector3(0, -15.0f, 0);
    public Vector3 yVelocity { get; set; } = Vector3.zero;
    public Vector3 xzVelocity { get; set; } = Vector3.zero;
    public Vector3 currentVelocity { get; set; } = Vector3.zero;


    public Vector3 distanceFromCameraAtJump { get; set; }

    public Collider[] groundColliders { get; set; } = new Collider[5];

    public Collider[] wallColliders { get; set; } = new Collider[10];

    private RaycastHit[] hits = new RaycastHit[10];

    #endregion

    private void Awake()
    {
    }

    // Dependency Injection of the most important player aspects
    public PlayerStateMachine(Rigidbody playerRb, CapsuleCollider playerCap, CameraController camera, Animator playerAnim, AnimationCurve AccelerationMultiplier)
    {
        this.playerRb = playerRb;
        this.playerCap = playerCap;
        this.camera = camera;
        this.playerAnim = playerAnim;
        this.AccelerationMultiplier = AccelerationMultiplier;
    }

    public PlayerState currentPlayerState { get; set; }

    public void Initialize(PlayerState startingState)
    {
        currentPlayerState = startingState;
        currentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        // Issues with current layout of our states, the root states would EXIT twice. If we are switching the state we exit, and then we also change state, we also exit.
        //currentPlayerState.ExitState();
        currentPlayerState = newState;
        currentPlayerState.EnterState();
    }

    public void Update()
    {
        float rawHorizontal = Input.GetAxisRaw("Horizontal");
        float rawVertical = Input.GetAxisRaw("Vertical");

        //if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) horizontalInput = 0;

        //if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) verticalInput = 0;

        // If any of the movement buttons are pressed, process
        if (rawHorizontal != 0f || rawVertical != 0f)
        {
            // If player is sprinting build up the input at a faster rate
            if (Input.GetKey(KeyCode.LeftShift))
            {
                horizontalInput += Time.deltaTime * rawHorizontal * 3f;
                //horizontalInput = (rawHorizontal > 0) ?
                //    Mathf.Clamp(horizontalInput, 0f, 2.0f) 
                //    :
                //    Mathf.Clamp(horizontalInput, -2.0f, 0f);
                horizontalInput = Mathf.Clamp(horizontalInput, -2.0f, 2.0f);

                verticalInput += Time.deltaTime * rawVertical * 3f;
                //verticalInput = (rawVertical > 0) ?
                //    Mathf.Clamp(verticalInput, 0f, 2.0f)
                //    :
                //    Mathf.Clamp(verticalInput, -2.0f, 0f);
                verticalInput = Mathf.Clamp(verticalInput, -2.0f, 2.0f);

            }
            // Else player is holding a direction button 
            // Need to be able to deccelerate when going from max sprint speed to walking speed instead of being instantly clamped 
            else
            {
                if (horizontalInput <= 1.0f && horizontalInput >= -1.0f)
                {
                    horizontalInput += Time.deltaTime * rawHorizontal * 2f;
                    //horizontalInput = (rawHorizontal > 0) ?
                    //    Mathf.Clamp(horizontalInput, 0f, 1.0f)
                    //    :
                    //    Mathf.Clamp(horizontalInput, -1.0f, 0f);
                    horizontalInput = Mathf.Clamp(horizontalInput, -1.0f, 1.0f);

                }
                if (verticalInput <= 1.0f && verticalInput >= -1.0f)
                {
                    verticalInput += Time.deltaTime * rawVertical * 2f;
                    //verticalInput = (rawVertical > 0) ?
                    //    Mathf.Clamp(verticalInput, 0f, 1.0f)
                    //    :
                    //    Mathf.Clamp(verticalInput, -1.0f, 0f);
                    verticalInput = Mathf.Clamp(verticalInput, -1.0f, 1.0f);
                }
                

                
            }
        } 


        // Input values will also need to be reset when we are NOT pressing an input button
        if (rawVertical == 0 || (verticalInput > 1.0f && !Input.GetKey(KeyCode.LeftShift) || (verticalInput < -1.0f && !Input.GetKey(KeyCode.LeftShift))))
        {
            if (verticalInput > 0)
            {
                verticalInput -= Time.deltaTime * decceleration;
                verticalInput = Mathf.Clamp(verticalInput, 0f, 2f);
            } else
            {
                //playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y, 0);
                verticalInput += Time.deltaTime * decceleration;
                verticalInput = Mathf.Clamp(verticalInput, -2f, 0f);

            }
        }
        if (rawHorizontal == 0 || (horizontalInput > 1.0f && !Input.GetKey(KeyCode.LeftShift)) || (horizontalInput < -1.0f && !Input.GetKey(KeyCode.LeftShift)))
        {
            if (horizontalInput > 0)
            {
                horizontalInput -= Time.deltaTime * decceleration;
                horizontalInput = Mathf.Clamp(horizontalInput, 0f, 2f);
            }
            else
            {
                //playerRb.velocity = new Vector3(0, playerRb.velocity.y,  playerRb.velocity.z);
                horizontalInput += Time.deltaTime * decceleration;
                horizontalInput = Mathf.Clamp(horizontalInput, -2f, 0f);

            }
        }

        // If player presses the lock on button: Lock on to nearest lock-onable entity
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Toggle lock on in CameraController and write down bool of status
            bool lockOnStatus = camera.toggleLockOn();
            //playerAnim.SetBool("isLockedOn", lockOnStatus);
            isLockedOn = lockOnStatus;

        }

        //playerAnim.SetFloat("horizontalInput", horizontalInput);
        //playerAnim.SetFloat("verticalInput", verticalInput);

        currentPlayerState.UpdateStates();
        
    }

    public void UpdatePhysicsStates()
    {
        currentSpeed = playerRb.velocity.magnitude;
        currentPlayerState.UpdatePhysicsStates();
    }

}
