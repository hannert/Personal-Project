using UnityEngine;

/// <summary>
/// The overarching Player script to be attached to the player GameObject
/// <para>Contains everything about the player..</para>
/// </summary>
public class Player : MonoBehaviour, IDamagable
{
    public float maxHealth { get ; set ; }
    public float currentHealth { get; set; }

    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    private Rigidbody playerRb;
    private CapsuleCollider playerCap;
    private new CameraController camera;
    private Animator playerAnim;

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
    public PlayerFallingState playerFallingState { get; set; }
    
    public PlayerJumpState playerJumpState { get; set; }

    #endregion

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();
        playerAnim = GetComponent<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();


        stateMachine = new PlayerStateMachine(playerRb, playerCap, camera, playerAnim);

        # region root states
        playerGroundedState = new PlayerGroundedState(this, stateMachine);
        playerAirState = new PlayerAirState(this, stateMachine);
        # endregion
        

        playerIdleState = new PlayerIdleState(this, stateMachine);
        playerWalkingState = new PlayerWalkingState(this, stateMachine);
        playerSprintingState = new PlayerSprintingState(this, stateMachine);

        playerFallingState = new PlayerFallingState(this, stateMachine);
        
        playerJumpState = new PlayerJumpState(this, stateMachine);

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



    }

    void FixedUpdate()
    {
        stateMachine.UpdatePhysicsStates();
    }
}
