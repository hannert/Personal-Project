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
    // State machine will have all the data?
    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float rollDistance = 4f;
    private float decceleration = 8f;
    public float skinWidth = 0.05f;

    public Rigidbody playerRb;
    public CapsuleCollider playerCap { get; set; }
    public CameraController camera { get; set; }

    public Animator playerAnim { get; set; }

    [SerializeField]
    public float horizontalInput { get; set; }
    [SerializeField]
    public float verticalInput { get; set; }

    #region Movement booleans
    // -----------------------
    public bool onGround { get; set; } = false;
    public bool canJump { get; set; } = true;
    public bool isJumping { get; set; } = false;
    public bool isFalling { get; set; } = false;
    public bool isWalking { get; set; } = false;
    public bool isSprinting { get; set; } = false;    
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

    public bool snapFlag { get; set; } = false;
    public float maxFallSpeed { get; set; } = 30.0f;
    public float currentFallVelocity { get; set; } = 0.0f;
    public float jumpVelocity { get; set; } = 10.0f;
    public float gravity { get; set; } = -15.0f;
    public float currentYPos { get; set; } = 0f;

    public Vector3 projectedPos { get; set; } = Vector3.zero;



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
        projectedPos = playerRb.position;
    }

    // Dependency Injection of the most important player aspects
    public PlayerStateMachine(Rigidbody playerRb, CapsuleCollider playerCap, CameraController camera, Animator playerAnim)
    {
        this.playerRb = playerRb;
        this.playerCap = playerCap;
        this.camera = camera;
        this.playerAnim = playerAnim;
    }

    public PlayerState currentPlayerState { get; set; }

    public void Initialize(PlayerState startingState)
    {
        currentPlayerState = startingState;
        currentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        currentPlayerState.ExitState();
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
                horizontalInput += Time.deltaTime * decceleration;
                horizontalInput = Mathf.Clamp(horizontalInput, -2f, 0f);

            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAnim.SetBool("isLockedOn", camera.toggleLockOn());
        }

        playerAnim.SetFloat("horizontalInput", horizontalInput);
        playerAnim.SetFloat("verticalInput", verticalInput);

        currentPlayerState.UpdateStates();
        
    }

    public void UpdatePhysicsStates()
    {
        currentPlayerState.UpdatePhysicsStates();
    }
}
