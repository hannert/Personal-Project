using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    bool goingToCrouch = false;
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
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
        if (!_psm.isCrouched)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                goingToCrouch = true;
                return;
            }
        }
    }

    public override void InitializeSubState()
    {
        // If player holds down crouch, enter crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _psm.checkGround = false;
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
        //if (!_psm.isCrouched)
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftControl))
        //    {
        //        SetSubState(player.playerCrouchState);
        //        return;
        //    }
        //}
        if (goingToCrouch == true)
        {
            SetSubState(player.playerCrouchState);
            goingToCrouch = false;
            return;
        }

        if (_psm.checkGround == true && goingToCrouch == false)
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
