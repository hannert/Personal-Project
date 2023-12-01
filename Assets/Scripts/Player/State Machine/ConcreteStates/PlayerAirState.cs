using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player in the air state, we apply gravity here
// Superstate
// -> In air and jumping
// -> In air and falling
public class PlayerAirState : PlayerState
{
    
    public PlayerAirState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override bool CheckSwitchStates()
    {
        if (playerStateMachine.onGround)
        {
            Debug.Log("Air TO ground switch");
            SwitchState(player.playerGroundedState);
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        playerStateMachine.onGround = false;
        InitializeSubState();
        Debug.Log("Entered Air State");
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        
    }

    public override void InitializeSubState()
    {
        // Is falling or ascending(Jumping)
        if (playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
        {
            Debug.Log("Set sub of air to idle");
            SetSubState(player.playerIdleState);
        }
        else
        {
            Debug.Log("Set sub of air to walking");
            SetSubState(player.playerWalkingState);
        }

    }

    public override void PhysicsUpdate()
    {
        if (PlayerUtilities.checkGroundCollision(playerStateMachine.groundColliders, playerStateMachine.playerCap) != 0 && playerStateMachine.currentFallVelocity < 0)
        {
            playerStateMachine.onGround = true;
            playerStateMachine.isJumping = false;
            playerStateMachine.canJump = true;
            playerStateMachine.isFalling = false;
            playerStateMachine.currentFallVelocity = 0;
        }
        if(CheckSwitchStates())
        {
            return;
        }
        Debug.Log("Modifying projected Pos in Air state");
        playerStateMachine.projectedPos = applyGravityToVector(playerStateMachine.playerRb.position);
        Debug.Log("Modifying projected Pos in Air state to " + playerStateMachine.projectedPos);

    }
    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        float newYPos = playerStateMachine.playerRb.transform.position.y + (playerStateMachine.currentFallVelocity * Time.fixedDeltaTime + ((0.5f) * playerStateMachine.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime));
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos, currentTrajectedPosition.z);
        playerStateMachine.currentFallVelocity += playerStateMachine.gravity * Time.fixedDeltaTime;
        playerStateMachine.currentFallVelocity = Mathf.Clamp(playerStateMachine.currentFallVelocity, -playerStateMachine.maxFallSpeed, playerStateMachine.jumpVelocity);
        return projectedPos;
    }

}
