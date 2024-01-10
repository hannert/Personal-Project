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
        _psm.playerRb.AddForce(Physics.gravity * 3, ForceMode.Acceleration);

        // Update if the player bool for is the player falling or not ( negative y velocity )
        if (_psm.playerRb.velocity.y < 0)
        {
            _psm.isFalling = true;
        }        

        if (CheckSwitchStates()) return;

    }
    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        Vector3 newPos = _psm.playerRb.position + (_psm.yVelocity * Time.fixedDeltaTime + ((0.5f) * _psm.gravityVector * Time.fixedDeltaTime * Time.fixedDeltaTime));
        _psm.yVelocity += _psm.gravityVector * Time.fixedDeltaTime;
        _psm.yVelocity = new Vector3(_psm.yVelocity.x, Mathf.Clamp(_psm.yVelocity.y, -_psm.maxFallSpeed, _psm.jumpVelocity), _psm.yVelocity.z);

        return newPos;
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

    }
}
