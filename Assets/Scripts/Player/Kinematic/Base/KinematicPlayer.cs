using UnityEngine;

/// <summary>
/// The overarching Player script to be attached to the player GameObject
/// <para>Contains everything about the player..</para>
/// </summary>
public class KinematicPlayer : MonoBehaviour, IDamagable
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

    public KinematicPlayerStateMachine stateMachine;

    #region Root States
    public KinematicPlayerGroundedState playerGroundedState { get; set; }
    public KinematicPlayerAirState playerAirState { get; set; }
   
    #endregion



    public KinematicPlayerIdleState playerIdleState { get; set; }
    public KinematicPlayerWalkingState playerWalkingState { get; set; }
    public KinematicPlayerSprintingState playerSprintingState { get; set; }
    public KinematicPlayerRollingState playerRollingState { get; set; }

    #region Combat States
    public KinematicPlayerAttackState playerAttackState { get; set; }
    public KinematicPlayerIdleWeaponState playerIdleWeaponState { get; set; }


    #endregion

    #endregion

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();
        playerAnim = GetComponent<Animator>();
        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();


        stateMachine = new KinematicPlayerStateMachine(playerRb, playerCap, camera, playerAnim);

        # region Root States
        playerGroundedState = new KinematicPlayerGroundedState(this, stateMachine);
        playerAirState = new KinematicPlayerAirState(this, stateMachine);
        #endregion

        #region Movement States
        playerIdleState = new KinematicPlayerIdleState(this, stateMachine);
        playerWalkingState = new KinematicPlayerWalkingState(this, stateMachine);
        playerSprintingState = new KinematicPlayerSprintingState(this, stateMachine);
        playerRollingState = new KinematicPlayerRollingState(this, stateMachine);
        #endregion

        #region Combat States
        playerAttackState = new KinematicPlayerAttackState(this, stateMachine);
        playerIdleWeaponState = new KinematicPlayerIdleWeaponState(this, stateMachine);
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



    }

    void FixedUpdate()
    {
        stateMachine.UpdatePhysicsStates();
    }
}
