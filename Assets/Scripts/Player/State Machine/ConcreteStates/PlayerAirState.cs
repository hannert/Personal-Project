using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Root State
/// <para>Player state if player has no ground beneath them</para>
/// ------ We apply gravity here ------
///
/// <para>-> In air and jumping</para>
/// <para>-> In air and falling</para>
/// </summary>
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

        CheckAnimationCondition();

        Debug.Log("Entered Air State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting air state!!!!!!!!!!!!!!!!!!!!!");
        // Normally exit to the Grounded State

        CheckAnimationCondition();

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

        

        if (CheckSwitchStates()) return;

        Debug.Log("Modifying projected Pos in Air state");
        playerStateMachine.projectedPos = applyGravityToVector(playerStateMachine.playerRb.position);

        UpdatePlayerBools();
        CheckAnimationCondition();


        Debug.Log("Modifying projected Pos in Air state to " + playerStateMachine.projectedPos);

    }
    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        float newYPos = playerStateMachine.playerRb.position.y + (playerStateMachine.currentFallVelocity * Time.fixedDeltaTime + ((0.5f) * playerStateMachine.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime));
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos, currentTrajectedPosition.z);
        playerStateMachine.currentFallVelocity += playerStateMachine.gravity * Time.fixedDeltaTime;
        playerStateMachine.currentFallVelocity = Mathf.Clamp(playerStateMachine.currentFallVelocity, -playerStateMachine.maxFallSpeed, playerStateMachine.jumpVelocity);
        Debug.Log(playerStateMachine.currentFallVelocity);
        Debug.Log(newYPos);
        return projectedPos;
    }

    private void CheckAnimationCondition()
    {
        if (playerStateMachine.onGround)
        {
            playerStateMachine.playerAnim.SetBool("isFalling", false);
            playerStateMachine.playerAnim.SetBool("isJumping", false);
            return;
        }

        // If not falling, ascending
        if (!isFalling())
        {
            playerStateMachine.playerAnim.SetBool("isFalling", false);
            playerStateMachine.playerAnim.SetBool("isJumping", true);

        }
        // Player is jumping up
        else
        {
            playerStateMachine.playerAnim.SetBool("isFalling", true);
            playerStateMachine.playerAnim.SetBool("isJumping", false);
        }
    }


    private bool isFalling()
    {
        return playerStateMachine.isFalling;
    }

    private void UpdatePlayerBools()
    {
        if (playerStateMachine.currentFallVelocity < 0)
        {
            playerStateMachine.isFalling = true;
            playerStateMachine.isJumping = false;
        } else
        {
            playerStateMachine.isFalling = false;
            playerStateMachine.isJumping = true;
        }
    }
}
