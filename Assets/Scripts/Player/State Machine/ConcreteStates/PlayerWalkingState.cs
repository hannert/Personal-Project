using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override bool CheckSwitchStates()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SwitchState(player.playerSprintingState);
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
        // Play animation
        Debug.Log("Entered Walking state");
        playerStateMachine.playerAnim.SetBool("isWalking", true);
    }

    public override void ExitState()
    {
        playerStateMachine.playerAnim.SetBool("isWalking", false);
    }

    public override void FrameUpdate()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void PhysicsUpdate()
    {
        // Get direction the player is already facing 
        var endDirection = PlayerUtilities.getDirectionFromOrigin(playerStateMachine.playerRb.rotation.eulerAngles.y);

        // Vector3 of proposed player position if player input being taken into account in relation to a camera
        var normalWalkPosition = 
            PlayerUtilities.GetDirectionFromCamera(playerStateMachine.projectedPos, playerStateMachine.camera.transform.position, playerStateMachine.horizontalInput, playerStateMachine.verticalInput);

        if (playerStateMachine.camera.isLockedOn)
        {
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(playerStateMachine.camera.lockOnFocusObject.transform.position, playerStateMachine.projectedPos, playerStateMachine.horizontalInput, playerStateMachine.verticalInput);
        }



        // TODO: Camera angle having an INSTANT effect on the player position is not desireable
        if (playerStateMachine.onGround)
        {
            // Based on Freya Holmers rotation vector video

            endDirection = normalWalkPosition;
        }
        if (!playerStateMachine.onGround)
        {
            // should be able to control the player a little bit when they are in the air
            // the strength of the rotation end direction should be dampened
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(playerStateMachine.projectedPos, playerStateMachine.projectedPos + playerStateMachine.distanceFromCameraAtJump, playerStateMachine.horizontalInput, playerStateMachine.verticalInput);
            Debug.Log(normalWalkPosition);
            endDirection = Vector3.Lerp(endDirection, normalWalkPosition, 0.2f);
        }

        // Apply the direction vector to the player position with speed 
        playerStateMachine.projectedPos += endDirection * playerStateMachine.speed * Time.fixedDeltaTime;

        // Get rotation of the player to reflect where the player is headed
        if (endDirection != Vector3.zero)
        {
            var directionOfMovement = Quaternion.LookRotation(endDirection, Vector3.up);
            playerStateMachine.playerRb.MoveRotation(directionOfMovement);
        }

        // Check if player is gonna collide into anything
        int wallCollisionNum = PlayerUtilities.checkWallCollision(playerStateMachine.wallColliders, playerStateMachine.playerCap, playerStateMachine.projectedPos);

        // Player will collide with a wall
        if (wallCollisionNum > 0)
        {
            playerStateMachine.projectedPos = PlayerUtilities.checkFuturePosition(
                endDirection, playerStateMachine.projectedPos, playerStateMachine.playerRb, playerStateMachine.playerCap, playerStateMachine.speed);
        }

        // Actually move the player
        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        CheckSwitchStates();
    }



}
