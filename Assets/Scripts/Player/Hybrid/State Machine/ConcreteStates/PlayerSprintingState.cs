using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintingState : PlayerWalkingState
{
    public PlayerSprintingState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
    }

    public override bool CheckSwitchStates()
    {


        if (!Input.GetKey(KeyCode.LeftShift))
        {
            SwitchState(player.playerWalkingState);
            return true;

        }
        if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            SwitchState(player.playerIdleState);
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=lime>Sprinting</color> State");
        _psm.isSprinting = true;
        _psm.speedMultiplier = 1.3f;
        //_psm.playerAnim.SetBool("isSprinting", true);
        
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=lime>Sprinting</color> State");

        _psm.isSprinting = false;
        _psm.speedMultiplier = 1.0f;
        //_psm.playerAnim.SetBool("isSprinting", false);

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
