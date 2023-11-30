using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerState
{
    public PlayerFallingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void CheckSwitchStates()
    {
        if (playerStateMachine.onGround)
        {
            SwitchState(player.playerGroundedState);
        }

    }

    public override void EnterState()
    {
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
        // While falling, check the ground if there is a surface
        checkGroundCollision();

        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        CheckSwitchStates();

    }




    private void checkGroundCollision()
    {
        Array.Clear(playerStateMachine.groundColliders, 0, playerStateMachine.groundColliders.Length);

        //We dont need point1 right now, but will be used if we need to use a capsule cast to reflect the players capsule collider size
        var localPoint1 = playerStateMachine.playerCap.center - Vector3.down * (playerStateMachine.playerCap.height / 2 - playerStateMachine.playerCap.radius);
        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint2 = playerStateMachine.playerCap.center + Vector3.down * (playerStateMachine.playerCap.height / 2 - playerStateMachine.playerCap.radius) * 1.1f; // Above point

        var point1 = player.transform.TransformPoint(localPoint1);
        var point2 = player.transform.TransformPoint(localPoint2);

        // Perhaps we can also try using a sphere overlap on the base of the player?
        DebugExtension.DebugWireSphere(point2, playerStateMachine.playerCap.radius, Time.fixedDeltaTime);
        int numColliders = Physics.OverlapSphereNonAlloc(point2, playerStateMachine.playerCap.radius, playerStateMachine.groundColliders, LayerMask.GetMask("Ground"));
        if (numColliders != 0)
        {
            playerStateMachine.onGround = true;
            playerStateMachine.isJumping = false;
            playerStateMachine.canJump = true;
            playerStateMachine.isFalling = false;
            playerStateMachine.currentFallVelocity = 0;
        }

    }

}
