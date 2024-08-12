using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintingState : PlayerWalkingState
{
    public PlayerSprintingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }

    public override bool CheckSwitchStates()
    {


        if (!Input.GetKey(KeyCode.LeftShift))
        {
            SwitchState(_factory.Walking());
            return true;

        }
        if (_ctx.HorizontalInput == 0 && _ctx.VerticalInput == 0)
        {
            SwitchState(_factory.Idle());
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=lime>Sprinting</color> State");
        _ctx.IsSprinting = true;
        _ctx.SpeedMultiplier = 1.3f;
        combatState = PlayerStateReq.SPRINTING;
        ComboEntryList = _ctx.GetCurrentWeapon().GetComponent<WeaponBase>().GetComboEntryKeys(combatState).ToArray();
        
        //_ctx.playerAnim.SetBool("isSprinting", true);
        
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=lime>Sprinting</color> State");

        _ctx.IsSprinting = false;
        _ctx.SpeedMultiplier = 1.0f;
        //_ctx.playerAnim.SetBool("isSprinting", false);

    }

    public override void AddForceToRB(Vector3 acceleration)
    {
        base.AddForceToRB(acceleration);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
