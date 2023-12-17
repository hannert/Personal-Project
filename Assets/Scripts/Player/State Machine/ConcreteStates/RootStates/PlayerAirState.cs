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
    
    public PlayerAirState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.onGround)
        {
            SwitchState(player.playerGroundedState);
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Air State");
        _psm.onGround = false;
        InitializeSubState();

        CheckAnimationCondition();

        
    }

    public override void ExitState()
    {
        Debug.Log("Exiting air state!!!!!!!!!!!!!!!!!!!!!");
        // Normally exit to the Grounded State

        CheckAnimationCondition();

    }

    public override void FrameUpdate()
    {
        
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
        if (PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) != 0 && _psm.currentFallVelocity < 0)
        {
            _psm.onGround = true;
            _psm.isJumping = false;
            _psm.canJump = true;
            _psm.isFalling = false;
            _psm.currentFallVelocity = 0;
        }

        

        if (CheckSwitchStates()) return;

        _psm.projectedPos = applyGravityToVector(_psm.playerRb.position);

        UpdatePlayerBools();
        CheckAnimationCondition();


        //Debug.Log("Modifying projected Pos in Air state to " + _psm.projectedPos);

    }
    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        float newYPos = _psm.playerRb.position.y + (_psm.currentFallVelocity * Time.fixedDeltaTime + ((0.5f) * _psm.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime));
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos, currentTrajectedPosition.z);
        _psm.currentFallVelocity += _psm.gravity * Time.fixedDeltaTime;
        _psm.currentFallVelocity = Mathf.Clamp(_psm.currentFallVelocity, -_psm.maxFallSpeed, _psm.jumpVelocity);
        return projectedPos;
    }

    private void CheckAnimationCondition()
    {
        if (_psm.onGround)
        {
            _psm.playerAnim.SetBool("isFalling", false);
            _psm.playerAnim.SetBool("isJumping", false);
            return;
        }

        // If not falling, ascending
        if (!isFalling())
        {
            _psm.playerAnim.SetBool("isFalling", false);
            _psm.playerAnim.SetBool("isJumping", true);

        }
        // Player is jumping up
        else
        {
            _psm.playerAnim.SetBool("isFalling", true);
            _psm.playerAnim.SetBool("isJumping", false);
        }
    }


    private bool isFalling()
    {
        return _psm.isFalling;
    }

    private void UpdatePlayerBools()
    {
        if (_psm.currentFallVelocity < 0)
        {
            _psm.isFalling = true;
            _psm.isJumping = false;
        } else
        {
            _psm.isFalling = false;
            _psm.isJumping = true;
        }
    }
}
