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
    private float exitTimerMaximum = 1.5f;


    bool startExitTimer = false;
    bool applyJump = false;
    bool exitFlag = false;

    bool switchToIdle = false;
    bool switchToWalk = false;


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
        if (Input.GetKeyUp(Keybinds.slide) || Input.GetKeyUp(Keybinds.slideAlt))
        {
            if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
            {
                // Set flag to switch back to idle state 
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
        _psm.isSliding = true;
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
        // If player's velocity is enough to be sliding, restart the timer!
        if (startExitTimer && _psm.playerRb.velocity.magnitude > minimumSlideSpeed)
        {
            // Debug.Log("Timer ended!");
            startExitTimer = false;

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

        // Raycast down to the ground to get the normal of the ground
        // We need to raycast from inside the player
        if (Physics.Raycast(_psm.playerRb.position + new Vector3(0, 0.3f), Vector3.down, out RaycastHit hit, 5.0f,LayerMask.GetMask("Ground"))){
            Debug.Log("Ground hit");
            Vector3 inputDirection = PlayerUtilities.GetInputDirection(_psm);

            // Check if the angle of the ground is sloping downward
            // To do that, we need to the get Vector parallel to the hit plane.
            // Project our intended player movement direction on the normal of the plane hit
            Vector3 ppp = Vector3.ProjectOnPlane(inputDirection, hit.normal);
            //Debug.DrawRay(_psm.playerRb.position, ppp, Color.yellow);
            
            // If the y-component of the projected vector is negative, then the player is trying to slide down a slope
            if (ppp.y < 0)
            {
                _psm.playerRb.AddForce(ppp * 70f, ForceMode.Force);
            }
        }

        //_psm.playerRb.AddForce(Vector3.down * 70f, ForceMode.Force);

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
