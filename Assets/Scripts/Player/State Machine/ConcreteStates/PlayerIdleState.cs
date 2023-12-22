using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.horizontalInput != 0 || _psm.verticalInput != 0)
        {

            // Left shift detected, go into sprint
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SwitchState(player.playerSprintingState);
                return true;
            }

            // Else go into normal walking
            else
            {
                SwitchState(player.playerWalkingState);
                return true;
            }
            
        }


        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Idle State");
        _psm.playerAnim.SetBool("isIdle", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        _psm.playerAnim.SetBool("isIdle", false);
    }

    public override void FrameUpdate()
    {
    }

    public override void InitializeSubState()
    {
        // Check if we have a weapon!
        // TODO: Further check for a weapon must be done later!
        if (_psm.isEquipped)
        {
            //Debug.Log("Substate for idle set to weapon idle");
            SetSubState(player.playerIdleWeaponState);
        }

    }

    public override void PhysicsUpdate()
    {
        // Idle would move if player is not pressing anything but also in the air!
        if (CheckSwitchStates())
        {
            return;
        }
        
        if (_psm.playerRb.transform.position == _psm.projectedPos)
        {
            return;
        }
        else
        {
            if(_psm.yVelocity.y > 0)
            {
                var ceilingVel = PlayerUtilities.collideAndSlide(_psm.playerCap, _psm.yVelocity.normalized, _psm.projectedPos, 0, _psm.skinWidth, 3, true, _psm.yVelocity.normalized, _psm.onGround);
                var newDirection = ceilingVel.normalized * _psm.yVelocity.magnitude;
                _psm.projectedPos = _psm.playerRb.position + (newDirection * Time.fixedDeltaTime + ((0.5f) * _psm.gravityVector * Time.fixedDeltaTime * Time.fixedDeltaTime));
            }
            




            _psm.playerRb.MovePosition(_psm.projectedPos);
        }
    }



    
}
