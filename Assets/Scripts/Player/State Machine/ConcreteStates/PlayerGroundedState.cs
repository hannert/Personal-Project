using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered grounded state");
        // Should snap the player to the ground here upon ENTERING
        // Send info to projectedPos
        playerStateMachine.onGround = true;
        InitializeSubState();
        Vector3 newPos = snapToGround(playerStateMachine.playerRb.position);
        playerStateMachine.projectedPos = newPos;
        Debug.Log("--" + playerStateMachine.projectedPos);
        //playerStateMachine.playerRb.MovePosition(snapToGround(playerStateMachine.playerRb.position));
        
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        getKeyPress();
    }

    public override void InitializeSubState()
    {
        // While grounded, not moving -> Idle
        if(playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
        {
            SetSubState(player.playerIdleState);
        } else
        {
            SetSubState(player.playerWalkingState);
        }
        

    }

    public override void PhysicsUpdate()
    {
        // When grounded, check if there is floor beneath to trigger falling state
        if(PlayerUtilities.checkGroundCollision(playerStateMachine.groundColliders, playerStateMachine.playerCap) == 0)
        {
            playerStateMachine.onGround = false;
        }
        CheckSwitchStates();
    }

   

    // Function should snap the player to the ground when its within a certain distance from landing
    // But what if we land on a SLANTED surface? -> Currently we keep teleporting up and falling down
    private Vector3 snapToGround(Vector3 currentTrajectedPosition)
    {

        float newYPos = getGroundPoint().y;
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos + 0.01f, currentTrajectedPosition.z);
        return projectedPos;

    }

    // Function should raycast from the player feet downwards to get the worldspace
    // coordinate of the surface they are standing on 
    private Vector3 getGroundPoint()
    {
        // We shoot a ray from the midpoint of the player to avoid faulty positions
        if (Physics.Raycast(playerStateMachine.playerRb.transform.position + new Vector3(0, playerStateMachine.playerCap.height / 2), Vector3.down, out RaycastHit hit, 5.0f, LayerMask.GetMask("Ground")))
        {
            //Debug.DrawRay(playerStateMachine.playerRb.transform.position, Vector3.down);
            Debug.DrawRay(hit.point, Vector3.up, Color.red, 3);
            Debug.Log(hit.point.y);
            return hit.point;
        }
        return Vector3.zero;
    }

    public override bool CheckSwitchStates()
    {
        if (playerStateMachine.onGround == false)
        {
            SwitchState(player.playerAirState);
            return true;
        }

        return false;

    }

    public void getKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerStateMachine.currentFallVelocity = playerStateMachine.jumpVelocity;
            SwitchState(player.playerAirState);
            Debug.Log("Space pressed");
        }
    }
}
