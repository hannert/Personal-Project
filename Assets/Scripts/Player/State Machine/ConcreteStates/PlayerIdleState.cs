using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void CheckSwitchStates()
    {
        if (playerStateMachine.horizontalInput != 0 || playerStateMachine.verticalInput != 0)
        {
            Debug.Log("Entering Walking substate");

            SwitchState(player.playerWalkingState);
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered Idle State");
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void PhysicsUpdate()
    {
        // Idle would move if player is not pressing anything but also in the air!
        CheckSwitchStates();
        if (playerStateMachine.playerRb.transform.position == playerStateMachine.projectedPos)
        {
            return;
        }
        else
        {
            Debug.Log("Moving in idle");
            playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        }
    }



    
}
