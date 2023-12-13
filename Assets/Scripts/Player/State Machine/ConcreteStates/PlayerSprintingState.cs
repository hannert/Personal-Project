using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintingState : PlayerWalkingState
{
    public PlayerSprintingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            SwitchState(player.playerWalkingState);
            return true;

        }
        if (playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
        {
            SwitchState(player.playerIdleState);
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Sprinting state");
        playerStateMachine.playerAnim.SetBool("isSprinting", true);
    }

    public override void ExitState()
    {
        playerStateMachine.playerAnim.SetBool("isSprinting", false);

    }

    private override Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed)
    {
        return projectedPosition + directionToMove * speed * 1.5f * Time.fixedDeltaTime;
    }
}
