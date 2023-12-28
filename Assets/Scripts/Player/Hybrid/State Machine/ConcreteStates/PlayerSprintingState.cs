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
        Debug.Log("Entered Sprinting state");
        _psm.isSprinting = true;
        _psm.playerAnim.SetBool("isSprinting", true);
    }

    public override void ExitState()
    {
        _psm.isSprinting = false;
        _psm.playerAnim.SetBool("isSprinting", false);

    }

    public override void AddForceToRB(Vector3 directionToMove, float speed)
    {
        _psm.playerRb.AddForce(directionToMove.normalized * speed * _psm.sprintModifier * Time.fixedDeltaTime * 4, ForceMode.VelocityChange);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
