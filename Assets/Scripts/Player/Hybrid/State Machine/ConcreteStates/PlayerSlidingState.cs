using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// State to denote that the player is currently sliding, should be appended to the crouch state and take the place of walking, idle.
/// </summary>
public class PlayerSlidingState : PlayerMovementState
{
    private float startYScale;
    private float minimumSlideSpeed = 3.0f;

    // Timer variables
    private float exitTimerElapsed = 0f;
    private float exitTimerMaximum = 3.0f;


    bool startExitTimer = false;
    bool applyJump = false;
    bool exitFlag = false;
    public PlayerSlidingState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
        startYScale = _psm.playerRb.transform.localScale.y;

    }

    public override void AddForceToRB(Vector3 acceleration)
    {
        
    }

    public override bool CheckSwitchStates()
    {
        // If player lets go of the slide key
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
            {
                SwitchState(player.playerIdleState);
                return true;
            }
            else
            {
                SwitchState(player.playerWalkingState);
                return true;
            }
        }
        return false;
    }

    // isSliding state is set from where it is entered from (Sprinting)
    public override void EnterState()
    {
        applyJump = false;
        exitFlag = false;
        startExitTimer = false;

        exitTimerElapsed = 0;

        //_psm.playerRb.drag = 3;


        base.EnterState();
    }

    public override void ExitState()
    {
        Debug.Log("Slide exit");
        _psm.isSliding = false;
    }

    public override void FrameUpdate()
    {
        // Should be interruptable
        ProcessInput();



        base.FrameUpdate();
    }

    public override void InitializeSubState()
    {
        base.InitializeSubState();
    }

    public override void PhysicsUpdate()
    {
        // Apply jump if possible
        if (applyJump)
        {
            _psm.playerRb.AddForce(Vector3.up * _psm.jumpForce, ForceMode.Impulse);
            exitFlag = true;
            SwitchState(player.playerAirState);
            return;
        }

        // If player's velocity is less than a small amount, start timer to leave state
        if (!startExitTimer && _psm.playerRb.velocity.magnitude < minimumSlideSpeed)
        {
            // Debug.Log("Timer started!");
            startExitTimer = true;

        }

        // TIME IS TICKING!! ⏰
        if (startExitTimer)
        {
            exitTimerElapsed += Time.fixedDeltaTime;
        }

        // TIMES UP!!
        if (exitTimerElapsed >= exitTimerMaximum)
        {
            if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
            {
                SwitchState(player.playerIdleState);
            }
            else
            {
                SwitchState(player.playerWalkingState);
            }

        }

        // raycast down and project on plane
        
        // Add more gravity 
        
    }

    public override void ProcessInput()
    {
        // If player jumps, leave sliding state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_psm.canJump == true)
            {
                applyJump = true;
            }
        }

        CheckSwitchStates();

    }

 
}
