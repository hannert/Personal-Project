using UnityEngine;

/// <summary>
/// The overarching Player script to be attached to the player GameObject
/// <para>Contains everything about the player..</para>
/// </summary>
public class Player : MonoBehaviour, IDamagable
{
    public float maxHealth { get ; set ; }
    public float currentHealth { get; set; }

    [Header("Movement")]
    
    [Tooltip("The players current speed")]
    public float currentSpeed;

    [Tooltip("The players maximum speed")]
    public float speed = 10.0f;

    [Tooltip("Curve for acceleration multiplier when turning the character in opposing directions")]   
    public AnimationCurve AccelerationMultiplier;

    [Tooltip("Sprint force modifier")]
    // TODO: Change this to a modifier rather than a set speed for sprint ?
    public float sprintSpeed = 15.0f;

    [Tooltip("Force added when player jumps")]
    public float jumpForce = 20.0f;
    

    // Internal Data for the player
    private Rigidbody playerRb;
    private CapsuleCollider playerCap;
    private new CameraController camera;
    private Animator playerAnim;

    
    // TODO: create combat system to deal with damage and health
    public void Damage(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Debug.Log("Player health 0");
    }


    #region State Machine

    public PlayerStateMachine stateMachine;

    #region Root States
    public PlayerGroundedState playerGroundedState { get; set; }
    public PlayerAirState playerAirState { get; set; }
   
    #endregion



    public PlayerIdleState playerIdleState { get; set; }
    public PlayerWalkingState playerWalkingState { get; set; }
    public PlayerSprintingState playerSprintingState { get; set; }
    public PlayerCrouchState playerCrouchState { get; set; }
    public PlayerSlidingState playerSlidingState { get; set; }
    public PlayerRollingState playerRollingState { get; set; }



    #region Combat States
    public PlayerAttackState playerAttackState { get; set; }
    public PlayerIdleWeaponState playerIdleWeaponState { get; set; }


    #endregion

    #endregion

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();
        playerAnim = GetComponentInChildren<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();

        stateMachine = new PlayerStateMachine(playerRb, playerCap, camera, playerAnim, AccelerationMultiplier);

        # region Root States
        playerGroundedState = new PlayerGroundedState(this, stateMachine, "Grounded");
        playerAirState = new PlayerAirState(this, stateMachine, "Air");
        #endregion

        #region Movement States
        playerIdleState = new PlayerIdleState(this, stateMachine, "Idle");
        playerWalkingState = new PlayerWalkingState(this, stateMachine, "Walking");
        playerSprintingState = new PlayerSprintingState(this, stateMachine, "Sprinting");
        playerCrouchState = new PlayerCrouchState(this, stateMachine, "Crouch");
        playerSlidingState = new PlayerSlidingState(this, stateMachine, "Sliding");
        playerRollingState = new PlayerRollingState(this, stateMachine, "Rolling");
        #endregion

        #region Combat States   
        playerAttackState = new PlayerAttackState(this, stateMachine, "-");
        playerIdleWeaponState = new PlayerIdleWeaponState(this, stateMachine, "-");
        #endregion

        stateMachine.Initialize(playerGroundedState);
    }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;


    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        stateMachine.speed = speed;
        stateMachine.jumpForce = jumpForce;

    }

    void FixedUpdate()
    {        
        currentSpeed = stateMachine.currentSpeed;
        stateMachine.UpdatePhysicsStates();
    }
}
