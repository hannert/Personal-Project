using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }

    public override bool CheckSwitchStates()
    {

        if (Input.GetKeyDown(Keybinds.primaryFire)) {
            SwitchState(_factory.Attacking(Keybinds.primaryFire));
            return true;
        }

        if (_ctx.horizontalInput != 0 || _ctx.verticalInput != 0)
        {

            // Left shift detected, go into sprint
            if (Input.GetKeyDown(Keybinds.sprint))
            {
                SwitchState(_factory.Sprinting());
                return true;
            }

            // Else go into normal walking
            else
            {
                SwitchState(_factory.Walking());
                return true;
            }
            
        }


        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=yellow>Idle</color> State");
        //_ctx.playerAnim.SetBool("isIdle", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=yellow>Idle</color> State");
        //_ctx.playerAnim.SetBool("isIdle", false);
    }

    public override void FrameUpdate()
    {
        if (CheckSwitchStates())
        {
            return;
        }
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_ctx.canJump == true)
            {
                // Switch to the air state ( So we avoid the grounded check again, allowing the player to double jump from a crouched position twice )

                _ctx.willJump = true;
                Debug.Log("Jumped");
                
            }
        }

    }

    public override void InitializeSubState()
    {
        // Check if we have a weapon!
        // TODO: Further check for a weapon must be done later!
        //if (_ctx.isEquipped)
        //{
        //    //Debug.Log("Substate for idle set to weapon idle");
        //    SetSubState(player.playerIdleWeaponState);
        //}

    }

    public override void PhysicsUpdate()
    {
    }



    
}
