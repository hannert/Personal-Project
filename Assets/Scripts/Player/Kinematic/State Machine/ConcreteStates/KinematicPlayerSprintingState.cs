using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPlayerSprintingState : KinematicPlayerWalkingState
{
    public KinematicPlayerSprintingState(KinematicPlayer player, KinematicPlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
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

    public override Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed)
    {
        return projectedPosition + directionToMove * speed * 1.5f * Time.fixedDeltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
