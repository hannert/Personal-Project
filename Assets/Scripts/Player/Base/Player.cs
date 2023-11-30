using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    public float maxHealth { get ; set ; }
    public float currentHealth { get; set; }

    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    private Rigidbody playerRb;
    private CapsuleCollider playerCap;
    private CameraController camera;


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
    public PlayerAirState playerAirState { get; set; }
    public PlayerGroundedState playerGroundedState { get; set; }
    #endregion

    public PlayerIdleState playerIdleState { get; set; }
    public PlayerFallingState playerFallingState { get; set; }
    public PlayerWalkingState playerWalkingState { get; set; }
    public PlayerJumpState playerJumpState { get; set; }

    #endregion

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();

        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();


        stateMachine = new PlayerStateMachine(playerRb, playerCap, camera);
        playerIdleState = new PlayerIdleState(this, stateMachine);
        playerGroundedState = new PlayerGroundedState(this, stateMachine);
        playerFallingState = new PlayerFallingState(this, stateMachine);
        playerWalkingState = new PlayerWalkingState(this, stateMachine);
        playerJumpState = new PlayerJumpState(this, stateMachine);
        playerAirState = new PlayerAirState(this, stateMachine);

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
