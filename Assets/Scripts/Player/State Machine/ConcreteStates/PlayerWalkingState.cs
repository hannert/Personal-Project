using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override void CheckSwitchStates()
    {
        if (playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
        {
            Debug.Log("Entering Idle substate");
            SwitchState(player.playerIdleState);
        }
    }

    public override void EnterState()
    {
        // Play animation
        Debug.Log("Entered Walking state");
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        // Take in input

    }

    public override void InitializeSubState()
    {
    }

    public override void PhysicsUpdate()
    {
        // Move player
        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(playerStateMachine.projectedPos.x, 0, playerStateMachine.projectedPos.z);
        var tempCamera = new Vector3(playerStateMachine.camera.transform.position.x, 0, playerStateMachine.camera.transform.position.z);

        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(playerStateMachine.horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, playerStateMachine.verticalInput);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        // Based on Freya Holmers rotation vector video
        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;

        playerStateMachine.projectedPos += endDirection * playerStateMachine.speed * Time.fixedDeltaTime;

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            playerStateMachine.playerRb.MoveRotation(testFinalRotation);
        }


        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);

        CheckSwitchStates();
    }





}
