using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hierarchal State Machine to be used by the Player
/// <para>Contains information on current state of the player movement</para>
/// </summary>
public class PlayerStateMachine
{
    // State machine will have all the data?
    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float rollDistance = 4f;
    public Rigidbody playerRb;
    public CapsuleCollider playerCap { get; set; }
    public CameraController camera { get; set; }

    public Animator playerAnim { get; set; }

    [SerializeField]
    public float horizontalInput { get; set; }
    [SerializeField]
    public float verticalInput { get; set; }

    // TODO : Seperate these into a state machine for modularity 
    public bool onGround { get; set; } = false;
    public bool canJump { get; set; } = true;
    public bool isJumping { get; set; } = false;
    public bool isFalling { get; set; } = false;
    public bool isSprinting { get; set; } = false;
    public bool snapFlag { get; set; } = false;

    public float maxFallSpeed { get; set; } = 30.0f;
    public float currentFallVelocity { get; set; } = 0.0f;
    public float jumpVelocity { get; set; } = 10.0f;
    public float gravity { get; set; } = -25.0f;
    public float currentYPos { get; set; } = 0f;

    public Vector3 projectedPos { get; set; }
    public Collider[] groundColliders { get; set; } = new Collider[5];

    public Collider[] wallColliders { get; set; } = new Collider[10];

    private RaycastHit[] hits = new RaycastHit[10];

    public PlayerStateMachine(Rigidbody playerRb, CapsuleCollider playerCap, CameraController camera, Animator playerAnim)
    {
        this.playerRb = playerRb;
        this.playerCap = playerCap;
        this.camera = camera;
        this.playerAnim = playerAnim;
    }

    



    #region getters/setters
    
    #endregion
    

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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) horizontalInput = 0;

        verticalInput = Input.GetAxisRaw("Vertical");
        //if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) verticalInput = 0;

        currentPlayerState.UpdateStates();
        
    }

    public void UpdatePhysicsStates()
    {
        currentPlayerState.UpdatePhysicsStates();
    }
}
