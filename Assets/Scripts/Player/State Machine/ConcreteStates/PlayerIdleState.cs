using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        if (playerStateMachine.horizontalInput != 0 || playerStateMachine.verticalInput != 0)
        {

            // Left shift detected, go into sprint
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                playerStateMachine.playerAnim.SetBool("isIdle", false);
                playerStateMachine.playerAnim.SetBool("isSprinting", true);
                return true;
            }

            // Else go into normal walking
            else
            {
                Debug.Log("Entering Walking substate");
                playerStateMachine.playerAnim.SetBool("isIdle", false);
                playerStateMachine.playerAnim.SetBool("isWalking", true);
                SwitchState(player.playerWalkingState);
                return true;
            }
            
        }

        return false;
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
        if (CheckSwitchStates())
        {
            return;
        }
        
        if (playerStateMachine.playerRb.transform.position == playerStateMachine.projectedPos)
        {
            return;
        }
        else
        {
            Debug.Log("Moving in idle to " + playerStateMachine.projectedPos);
            playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        }
    }



    
}
