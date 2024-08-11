using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float CurrentSpeed { get; set; } = 0f;

    /// <summary>
    /// Speed of the player to move at, Deprecated since acceleration will be used but this is currently used in determining roll distance
    /// </summary>
    public float Speed { get; set; } = 20.0f;

    /// <summary>
    /// A multiplier to go on top of the movement functions in the future (Faster/slower from skills and or status effects)
    /// </summary>
    public float SpeedMultiplier { get; set; } = 1f;

    /// <summary>
    /// The flat speed multiplier when player is sprinting
    /// </summary>
    public float SprintModifier { get; set; } = 1.5f; 

    /// <summary>
    /// The acceleration the player will have
    /// </summary>
    public float Acceleration { get; set; } = 100f;

    /// <summary>
    ///  The maximum acceleration the player CAN have
    /// </summary>
    public float MaxAcceleration { get; set; } = 80f;

    /// <summary>
    ///  The amount of force the impulse will exert on the player when jumping
    /// </summary>
    public float JumpForce { get; set; } = 20.0f;

    /// <summary>
    /// The decceleration of input damping (NOT FOR MOVEMENT)
    /// </summary>
    private float Decceleration { get; }= 8f;

    /// <summary>
    /// The boundary between the player and collision detection services
    /// </summary>
    public float skinWidth = 0.05f;


    #region Components ------

    /// <summary>
    /// Reference to the players RigidBody component
    /// </summary>
    public Rigidbody PlayerRb { get; private set; }

    /// <summary>
    /// Reference to the players CapsuleCollider component
    /// </summary>
    public CapsuleCollider PlayerCap { get; set; }

    /// <summary>
    /// Reference to the camera system
    /// </summary>
    public CameraController Camera { get; set; }

    /// <summary>
    /// Reference to the players Animator component
    /// </summary>
    public Animator PlayerAnim { get; set; }

    ///<summary>
    /// Reference to the players runtime animator component
    /// </summary>
    public RuntimeAnimatorController RuntimePlayerAnim { get; set; }

    /// <summary>
    /// Reference to the players runtime animator override controller component
    /// </summary>
    public AnimatorOverrideController AnimatorOverrideController { get; set; }

    public CustomAnimationClass.AnimationClipOverrides ClipOverrides {get; set;}

    #endregion


    /// <summary>
    /// The current amount of horizontal input: a, d
    /// </summary>
    public float HorizontalInput { get; set; }

    /// <summary>
    /// The current amount of vertical input: w, s
    /// </summary>
    public float VerticalInput { get; set; }

    /// <summary>
    /// maximum number of jumps the player can take after consuming the regular jump 
    /// </summary>
    public float ExtraJumpsMax { get; set; } = 1;

    /// <summary>
    /// number of extra jumps the player has taken
    /// </summary>
    public float ExtraJumpsTaken { get; set; } = 0;

    #region Movement booleans
    /// <summary>
    /// boolean to denote whether or not player is touching the ground or not
    /// </summary>
    public bool OnGround { get; set; } = false;

    /// <summary>
    /// boolean to denote whether or not player can jump (Dependent if extraJumpsTaken is less than extraJumpsMax)
    /// </summary>
    public bool CanJump { get; set; } = true;

    /// <summary>
    /// boolean to denote whether the player will jump in the current Physics update cycle
    /// </summary>
    public bool WillJump { get; set; } = false;

    /// <summary>
    /// boolean to keep track if the player CONSUMED regular jump ( Seperate from extra jumps from double jump )
    /// true == player used reg jump
    /// false == player can still jump
    /// </summary>
    public bool RegJumpTaken { get; set; } = false;

    /// <summary>
    /// deprecated variable to keep track of player having pos y velocity
    /// </summary>
    public bool IsJumping { get; set; } = false;

    /// <summary>
    /// boolean of player falling ( neg y velocity )
    /// </summary>
    public bool IsFalling { get; set; } = false;

    /// <summary>
    /// boolean of player walking ( entering the walking state )
    /// </summary>
    public bool IsWalking { get; set; } = false;

    /// <summary>
    ///  boolean of player sprinting ( entering the sprinting state )
    /// </summary>
    public bool IsSprinting { get; set; } = false;

    /// <summary>
    /// boolean of player crouching ( entering the crouched state )
    /// </summary>
    public bool IsCrouched { get; set; } = false;

    /// <summary>
    ///  boolean of player sliding ( entering sliding state )
    /// </summary>
    public bool IsSliding { get; set; } = false;

    /// <summary>
    /// boolean of if player is locked onto an enemy entity
    /// </summary>
    public bool IsLockedOn { get; set; } = false;

    /// <summary>
    /// boolean of player rolling
    /// </summary>
    public bool IsRolling { get; set; } = false;

    /// <summary>
    /// boolean of if player is colliding with a wall and having neg y velocity
    /// </summary>
    public bool IsWallSliding { get; set; } = false;
    #endregion

    #region Combat

    /// <summary>
    /// boolean of if player is in the attacking state or not
    /// </summary>
    public bool IsAttacking { get; set; } = false;

    /// <summary>
    /// does the player have a weapon?
    /// </summary>
    public bool HasWeapon { get; set; } = false;

    /// <summary>
    /// does the player have a shield?
    /// </summary>
    public bool HasShield { get; set; } = false;

    /// <summary>
    /// Is the weapon equipped?
    /// </summary>
    public bool IsEquipped { get; set; } = true;


    // TODO: This is only acceptable with spawning in with the weapon when playmode is entered, need a better way of changing it
    /// <summary>
    /// The GameObject to spawn with the player 
    /// </summary>
    private GameObject CurrentWeapon { get; set; }

    private AnimationClip PrevAttackClip { get; set; } = null;

    private String PrevAttackName { get; set; } = "";

    public GameObject GetCurrentWeapon() {
        //Debug.Log("Get current weapon");
        if (CurrentWeapon == null) {
            //Debug.Log("no weapon on player");
        }
        return CurrentWeapon;
    }

    public void SetCurrentWeapon(GameObject newWeapon) {
        this.CurrentWeapon = newWeapon;
    }

    public void SwapWeapon(GameObject newWeapon) {
        SetCurrentWeapon(newWeapon);

    }

    public void SetAttackAnimation(AnimationClip newAnim) {
        //Debug.Log("Setting new attack animation ----");        
        PrevAttackClip = newAnim;
        PrevAttackName = newAnim.name;
        newAnim.name = "BlankWeapon";
        AnimatorOverrideController["BlankWeapon"] = newAnim;
        AnimatorOverrideController["BlankWeapon"].wrapMode = WrapMode.Once;

        PlayAttackAnimation();
        
        //newAnim.name = initName;
    }

    public void PlayAttackAnimation() {
        //Debug.Log("Playing attack animation");
        PlayerAnim.Play("OneShot", -1, 0f);
    }

    // TODO - Currently, the player animations for combat has to be renamed to a universal string, but that breaks the animator tree when editing them
    public void ResetPrevAnimName() {

    }

    #endregion


    /// <summary>
    /// The array of Colliders used to check if player is in contact with solid ground
    /// </summary>
    public Collider[] GroundColliders { get; set; } = new Collider[5];

    /// <summary>
    /// The array of Colliders used to check which walls the player is in contact with
    /// </summary>
    public Collider[] WallColliders { get; set; } = new Collider[10];

    private RaycastHit[] Hits = new RaycastHit[10];

    #endregion

    #region State Machine

    /// <summary>
    /// The current state the player is in
    /// </summary>
    public PlayerState CurrentPlayerState { get; set; }

    // Dependency Injection of the most important player aspects
    public PlayerStateMachine(Rigidbody playerRb, CapsuleCollider playerCap, CameraController camera, Animator playerAnim, AnimationCurve AccelerationMultiplier)
    {
        this.PlayerRb = playerRb;
        this.PlayerCap = playerCap;
        this.Camera = camera;
        this.PlayerAnim = playerAnim;
        SetUpAnimator();

        this.AccelerationMultiplier = AccelerationMultiplier;
    }

    void SetUpAnimator(){

        AnimatorOverrideController = new AnimatorOverrideController(PlayerAnim.runtimeAnimatorController);
        PlayerAnim.runtimeAnimatorController = AnimatorOverrideController;

    }



    public void Initialize(PlayerState startingState)
    {
        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        // Issues with current layout of our states, the root states would EXIT twice. If we are switching the state we exit, and then we also change state, we also exit.
        //currentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }

    /// <summary>
    /// Function to change the current root state without altering a re-initialization of substates
    /// </summary>
    public void SwitchRootState(PlayerState newState, PlayerState subState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;

        // We set a variable to bypass the InitializeSubState function
        CurrentPlayerState.EnterState();

        // Manually set the substate to the new root state
        CurrentPlayerState.SwitchSubState(subState);
    }

    #endregion


    #region Update
    public void Update()
    {
        float rawHorizontal = Input.GetAxisRaw("Horizontal");
        float rawVertical = Input.GetAxisRaw("Vertical");

        // If any of the movement buttons are pressed, process
        if (rawHorizontal != 0f || rawVertical != 0f)
        {
            // If player is sprinting build up the input at a faster rate
            if (Input.GetKey(KeyCode.LeftShift))
            {
                HorizontalInput += Time.deltaTime * rawHorizontal * 3f;
                HorizontalInput = Mathf.Clamp(HorizontalInput, -2.0f, 2.0f);
                VerticalInput += Time.deltaTime * rawVertical * 3f;
                VerticalInput = Mathf.Clamp(VerticalInput, -2.0f, 2.0f);

            }
            // Else player is holding a direction button 
            // Need to be able to deccelerate when going from max sprint speed to walking speed instead of being instantly clamped 
            else
            {
                if (HorizontalInput <= 1.0f && HorizontalInput >= -1.0f)
                {
                    HorizontalInput += Time.deltaTime * rawHorizontal * 2f;
                    HorizontalInput = Mathf.Clamp(HorizontalInput, -1.0f, 1.0f);

                }
                if (VerticalInput <= 1.0f && VerticalInput >= -1.0f)
                {
                    VerticalInput += Time.deltaTime * rawVertical * 2f;
                    VerticalInput = Mathf.Clamp(VerticalInput, -1.0f, 1.0f);
                }
                

                
            }
        } 


        // Input values will also need to be reset when we are NOT pressing an input button
        if (rawVertical == 0 || (VerticalInput > 1.0f && !Input.GetKey(KeyCode.LeftShift) || (VerticalInput < -1.0f && !Input.GetKey(KeyCode.LeftShift))))
        {
            if (VerticalInput > 0)
            {
                VerticalInput -= Time.deltaTime * Decceleration;
                VerticalInput = Mathf.Clamp(VerticalInput, 0f, 2f);
            } else
            {
                VerticalInput += Time.deltaTime * Decceleration;
                VerticalInput = Mathf.Clamp(VerticalInput, -2f, 0f);

            }
        }
        if (rawHorizontal == 0 || (HorizontalInput > 1.0f && !Input.GetKey(KeyCode.LeftShift)) || (HorizontalInput < -1.0f && !Input.GetKey(KeyCode.LeftShift)))
        {
            if (HorizontalInput > 0)
            {
                HorizontalInput -= Time.deltaTime * Decceleration;
                HorizontalInput = Mathf.Clamp(HorizontalInput, 0f, 2f);
            }
            else
            {
                HorizontalInput += Time.deltaTime * Decceleration;
                HorizontalInput = Mathf.Clamp(HorizontalInput, -2f, 0f);

            }
        }

        // If player presses the lock on button: Lock on to nearest lock-onable entity
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Toggle lock on in CameraController and write down bool of status
            bool lockOnStatus = Camera.toggleLockOn();
            //playerAnim.SetBool("isLockedOn", lockOnStatus);
            IsLockedOn = lockOnStatus;

        }

        // Get input for combat
        // TODO - Check if input can be taken combat-wise?
        if (!IsAttacking) {
            // foreach(var combatKey in currentWeapon.moveset_get())
        }
        

        CurrentPlayerState.UpdateStates();
        
    }

    public void UpdatePhysicsStates()
    {
        // Update currentSpeed for debugging purposes
        CurrentSpeed = PlayerRb.velocity.magnitude;
        CurrentPlayerState.UpdatePhysicsStates();
    }

    #endregion
}
