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
            if (Input.GetKeyDown(Keybinds.sprint))
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
        //_psm.playerAnim.SetBool("isIdle", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Idle state");
        //_psm.playerAnim.SetBool("isIdle", false);
    }

    public override void FrameUpdate()
    {
        if (CheckSwitchStates())
        {
            return;
        }
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_psm.canJump == true)
            {
                // Switch to the air state ( So we avoid the grounded check again, allowing the player to double jump from a crouched position twice )

                _psm.willJump = true;
                Debug.Log("Jumped");
                
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
