using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    bool checkGround = true;
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered grounded state");
        _psm.playerAnim.SetBool("isGrounded", true);
        _psm.onGround = true;
        _psm.playerRb.drag = 5;
        InitializeSubState();
        
    }

    public override void ExitState()
    {
        Debug.Log("Exited grounded root state");
        _psm.onGround = false;
        _psm.playerAnim.SetBool("isGrounded", false);
        _psm.playerRb.drag = 1;

    }

    public override void FrameUpdate()
    {

    }

    public override void InitializeSubState()
    {
        // If player holds down crouch, enter crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetSubState(player.playerCrouchState);
        }

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
        if (!_psm.isCrouched)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                SetSubState(player.playerCrouchState);
                return;
            }
        }
        if (checkGround)
        {
            // When grounded, check if there is floor beneath to trigger falling state
            if (PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) == 0)
            {
                _psm.onGround = false;
            }
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

}
