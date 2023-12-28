using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
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
        Debug.Log("Exited Idle state");
        _psm.playerAnim.SetBool("isIdle", false);
    }

    public override void FrameUpdate()
    {
        if (CheckSwitchStates())
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_psm.canJump == true)
            {
                Debug.Log("Jumped");
                _psm.playerRb.AddForce(Vector3.up * _psm.jumpForce, ForceMode.Impulse);
                _psm.canJump = false;
            }
        }
    }

    public override void InitializeSubState()
    {
        // Check if we have a weapon!
        // TODO: Further check for a weapon must be done later!
        //if (_psm.isEquipped)
        //{
        //    //Debug.Log("Substate for idle set to weapon idle");
        //    SetSubState(player.playerIdleWeaponState);
        //}

    }

    public override void PhysicsUpdate()
    {
    }



    
}
