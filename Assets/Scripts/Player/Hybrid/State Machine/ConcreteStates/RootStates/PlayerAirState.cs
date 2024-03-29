using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Root State
/// <para>Player state if player has no ground beneath them</para>
/// ------ We apply gravity here ------
///
/// <para>-> In air and jumping</para>
/// <para>-> In air and falling</para>
/// </summary>
public class PlayerAirState : PlayerState
{
    // Timer for coyote time allowable jumps
    float currentTimer = 0f;
    float maxCoyoteTime = 0.16f;

    public PlayerAirState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
        _isRootState = true;
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.isFalling == true)
        {
            if (PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) != 0)
            {
                Debug.Log("Ground collision returned true in air state");
                SwitchState(player.playerGroundedState);
                return true;
            }
        }
        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Air State");
        _psm.onGround = false;
        InitializeSubState();        
    }

    public override void ExitState()
    {
        Debug.Log("Exiting air state!!!!!!!!!!!!!!!!!!!!!");
        // Normally exit to the Grounded State
        _psm.isJumping = false;
        _psm.isFalling = false;
        currentTimer = 0f;
    }

    public override void FrameUpdate()
    {
        // Check for player input 
        // Check if jump key is pressed (Coyote time)


        // Get input whether or not player should slide when hitting the ground!

        
    }

    public override void InitializeSubState()
    {
        // Is falling or ascending(Jumping)
        if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            Debug.Log("Set sub of air to idle");
            SetSubState(player.playerIdleState);
        }
        else
        {
            Debug.Log("Set sub of air to walking");
            SetSubState(player.playerWalkingState);
        }

    }

    public override void PhysicsUpdate()
    {
        
       

        if (currentTimer <= maxCoyoteTime)
        {
            Debug.Log("Logging");
            // Add to the coyote time timer
            currentTimer += Time.fixedDeltaTime;
            if (_psm.willJump)
            {
                _psm.playerRb.AddForce(Vector3.up * _psm.jumpForce, ForceMode.Impulse);
                _psm.canJump = false;
                _psm.willJump = false;
            }


        }
        


        _psm.playerRb.AddForce(Physics.gravity * 3, ForceMode.Acceleration);

        // Update if the player bool for is the player falling or not ( negative y velocity )
        if (_psm.playerRb.velocity.y < 0)
        {
            _psm.isFalling = true;
        }        

        if (CheckSwitchStates()) return;

    }


}
