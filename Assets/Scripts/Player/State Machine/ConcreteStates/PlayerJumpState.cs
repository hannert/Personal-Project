using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        Debug.Log("Entered Jump State");
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
    }

    public override void InitializeSubState()
    {
        // Could move mid air
        // Add a sub state of idle or walking for now? (i think it will explode)

    }

    public override void PhysicsUpdate()
    {
        // Player is ASCENDING
        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);

    }


    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        float newYPos = playerStateMachine.currentYPos + (playerStateMachine.currentFallVelocity * Time.fixedDeltaTime + ((0.5f) * playerStateMachine.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime));
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos, currentTrajectedPosition.z);
        playerStateMachine.currentFallVelocity += playerStateMachine.gravity * Time.fixedDeltaTime;
        playerStateMachine.currentFallVelocity = Mathf.Clamp(playerStateMachine.currentFallVelocity, -playerStateMachine.maxFallSpeed, playerStateMachine.jumpVelocity);
        return projectedPos;
    }

}
