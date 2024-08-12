using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }

    private CombatBindsEnum[] ComboEntryList;

    public override bool CheckSwitchStates()
    {
        // Check for player input combat-wise,
        // If our movesets' condition matches with IDLE, then poll for input to enter a combo


        CombatBindsEnum tempBind;
        for (int i = 0; i < ComboEntryList.Length; i++) {
            tempBind = ComboEntryList[i];
            if (Input.GetKeyDown(CombatBinds.enumToCode[tempBind])) {
                if (_ctx.OnGround) {
                    // Go into the grounded variation

                }
                else if (!_ctx.OnGround) {
                    // Go into the airborne variation

                }

                SwitchState(_factory.Attacking(CombatBinds.enumToCode[tempBind], PlayerStateReq.IDLE));
            }
        }


        if (_ctx.HorizontalInput != 0 || _ctx.VerticalInput != 0)
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

        ComboEntryList = _ctx.GetCurrentWeapon().GetComponent<WeaponBase>().GetComboEntryKeys(PlayerStateReq.IDLE).ToArray();


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
