using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWalkingState : PlayerMovementState
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
        // While in walking
        // ------------------------
        // Grounded walking
        // Grounded locked on walking
        // Air walking
        // Air locked on walking
        

        // Direction the player is already facing 
        var endDirection = Vector3.zero;
        // Vector3 of proposed player position if player input being taken into account in relation to a camera
        var normalWalkPosition = Vector3.zero;


        // Player is on the ground
        if (playerStateMachine.onGround)
        {
            var directionOfPlayerForwardRotation = PlayerUtilities.getDirectionFromOrigin(playerStateMachine.playerRb.rotation.eulerAngles.y);
            normalWalkPosition =  PlayerUtilities.GetDirectionFromCamera(playerStateMachine.projectedPos, playerStateMachine.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            endDirection = normalWalkPosition;
        }


        // Player is NOT on the ground
        if (!playerStateMachine.onGround)
        {
            // should be able to control the player a little bit when they are in the air
            // the strength of the rotation end direction should be dampened
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(
                playerStateMachine.projectedPos, playerStateMachine.projectedPos + playerStateMachine.distanceFromCameraAtJump, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            endDirection = Vector3.Lerp(endDirection, normalWalkPosition, 1f);
        }


        #region Calculate the position the player will move to
        // If the player is not locked on, proceed with movement based on where the camera is looking.
        // If player IS locked on, move the player based on an axis based on the Vector3 of the LockOnTarget and the Player rather than the camera.
        if (!playerStateMachine.camera.isLockedOn)
        {
            Debug.Log("WOWW!!!");
            // Apply the direction vector to the player position with speed 
            playerStateMachine.projectedPos = CalculatePositionToMoveTo(playerStateMachine.projectedPos, endDirection, playerStateMachine.speed);
        } else
        {
            
            // Get direction with axes of LockOnTarget and Player
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(playerStateMachine.camera.lockOnFocusObject.transform.position, playerStateMachine.projectedPos, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            playerStateMachine.projectedPos = CalculatePositionToMoveTo(playerStateMachine.projectedPos, normalWalkPosition, playerStateMachine.speed);

        }
        #endregion

        #region Rotate the players model
        // Get rotation of the player to reflect where the player is headed
        if (endDirection != Vector3.zero)
        {
            if (!playerStateMachine.camera.isLockedOn)
            {
                var directionOfMovement = Quaternion.LookRotation(endDirection, Vector3.up);
                playerStateMachine.playerRb.MoveRotation(directionOfMovement);
            } else
            {
                // If locked on, look at the object that is locked onto
                var directionToLockOn = Quaternion.LookRotation(playerStateMachine.camera.lockOnFocusObject.transform.position - playerStateMachine.playerRb.position, Vector3.up);
                playerStateMachine.playerRb.MoveRotation(directionToLockOn);
            }
            
        }
        #endregion

        #region Wall collision 
        // Check if player is gonna collide into anything
        int wallCollisionNum = PlayerUtilities.checkWallCollision(playerStateMachine.wallColliders, playerStateMachine.playerCap, playerStateMachine.projectedPos);

        // Player will collide with a wall
        if (wallCollisionNum > 0)
        {
            playerStateMachine.projectedPos = PlayerUtilities.checkFuturePosition(
                endDirection, playerStateMachine.projectedPos, playerStateMachine.playerRb, playerStateMachine.playerCap, playerStateMachine.speed);
        }
        #endregion

        // Actually move the player
        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        CheckSwitchStates();
    }

    public override Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed)
    {
        return projectedPosition + directionToMove * speed * Time.fixedDeltaTime;
    }


}
