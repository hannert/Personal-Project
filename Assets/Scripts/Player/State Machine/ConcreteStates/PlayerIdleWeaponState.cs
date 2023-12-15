using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleWeaponState : PlayerState
{

    public PlayerIdleWeaponState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwitchState(player.playerAttackState);
            return true;
        }


        return false;
    }

    public override void EnterState()
    {


    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        CheckSwitchStates();
    }

    public override void PhysicsUpdate()
    {
    }
}
