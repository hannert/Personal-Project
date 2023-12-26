using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered grounded state");
        _psm.playerAnim.SetBool("isGrounded", true);
        _psm.onGround = true;
        InitializeSubState();
        
    }

    public override void ExitState()
    {
        Debug.Log("Exited grounded root state");
        _psm.onGround = false;
        _psm.playerAnim.SetBool("isGrounded", false);

    }

    public override void FrameUpdate()
    {
        getKeyPress();
    }

    public override void InitializeSubState()
    {
        // While grounded, not moving -> Idle
        if(_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            SetSubState(player.playerIdleState);
        } else
        {
            SetSubState(player.playerWalkingState);
        }
        

    }

    public override void PhysicsUpdate()
    {

        // When grounded, check if there is floor beneath to trigger falling state
        if (KinematicPlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) == 0)
        {
            _psm.onGround = false;
        }
        CheckSwitchStates();
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.onGround == false)
        {
            SwitchState(player.playerAirState);
            return true;
        }

        return false;

    }

    public void getKeyPress()
    {
        // Jump from grounded state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchState(player.playerAirState);
        }
        // Roll from grounded state

    }
}
