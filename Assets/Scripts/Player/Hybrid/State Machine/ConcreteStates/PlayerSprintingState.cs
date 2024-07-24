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
        if (_ctx.horizontalInput == 0 && _ctx.verticalInput == 0)
        {
            SwitchState(_factory.Idle());
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=lime>Sprinting</color> State");
        _ctx.isSprinting = true;
        _ctx.speedMultiplier = 1.3f;
        //_ctx.playerAnim.SetBool("isSprinting", true);
        
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=lime>Sprinting</color> State");

        _ctx.isSprinting = false;
        _ctx.speedMultiplier = 1.0f;
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
