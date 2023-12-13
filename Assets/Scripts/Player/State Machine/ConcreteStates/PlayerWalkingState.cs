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
        if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
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
        _psm.isWalking = true;
        _psm.playerAnim.SetBool("isWalking", true);
    }

    public override void ExitState()
    {
        _psm.isWalking = false;
        _psm.playerAnim.SetBool("isWalking", false);
    }

    public override void FrameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {

            SwitchState(player.playerRollingState);
        }
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
        if (_psm.onGround)
        {
            var directionOfPlayerForwardRotation = PlayerUtilities.getDirectionFromOrigin(_psm.playerRb.rotation.eulerAngles.y);
            normalWalkPosition =  PlayerUtilities.GetDirectionFromCamera(_psm.projectedPos, _psm.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            endDirection = normalWalkPosition;
        }


        // Player is NOT on the ground
        if (!_psm.onGround)
        {
            // should be able to control the player a little bit when they are in the air
            // the strength of the rotation end direction should be dampened
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(
                _psm.projectedPos, _psm.projectedPos + _psm.distanceFromCameraAtJump, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            endDirection = Vector3.Lerp(endDirection, normalWalkPosition, 1f);
        }


        #region Calculate the position the player will move to
        // If the player is not locked on, proceed with movement based on where the camera is looking.
        // If player IS locked on, move the player based on an axis based on the Vector3 of the LockOnTarget and the Player rather than the camera.
        if (!_psm.camera.isLockedOn)
        {
            Debug.Log("WOWW!!!");
            // Apply the direction vector to the player position with speed 
            _psm.projectedPos = CalculatePositionToMoveTo(_psm.projectedPos, endDirection, _psm.speed);
        } else
        {

            // Get direction with axes of LockOnTarget and Player
            normalWalkPosition = PlayerUtilities.GetDirectionFromCamera(_psm.camera.lockOnFocusObject.transform.position, _psm.projectedPos, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _psm.projectedPos = CalculatePositionToMoveTo(_psm.projectedPos, normalWalkPosition, _psm.speed);

        }
        #endregion

        #region Rotate the players model
        // Get rotation of the player to reflect where the player is headed
        if (endDirection != Vector3.zero)
        {
            // If player is NOT locked on
            if (!_psm.camera.isLockedOn)
            {
                var directionOfMovement = Quaternion.LookRotation(endDirection, Vector3.up);
                _psm.playerRb.MoveRotation(directionOfMovement);
            } else
            {
                // If locked on, look at the object that is locked onto]
                // Currently, we only want the Y rotation component from the LookRotation. 
                // !!! TODO: Perhaps we want all of the rotation for in-air movement and further states
                var directionToLockOn = Quaternion.LookRotation(_psm.camera.lockOnFocusObject.transform.position - _psm.playerRb.position, Vector3.up);
                directionToLockOn = Quaternion.Euler(0, directionToLockOn.eulerAngles.y, 0);
                _psm.playerRb.MoveRotation(directionToLockOn);
            }
            
        }
        #endregion

        #region Wall collision 
        // Check if player is gonna collide into anything
        int wallCollisionNum = PlayerUtilities.checkWallCollision(_psm.wallColliders, _psm.playerCap, _psm.projectedPos);

        // Player will collide with a wall
        if (wallCollisionNum > 0)
        {
            _psm.projectedPos = PlayerUtilities.checkFuturePosition(
                endDirection, _psm.projectedPos, _psm.playerRb, _psm.playerCap, _psm.speed);
        }
        #endregion

        // Actually move the player
        _psm.playerRb.MovePosition(_psm.projectedPos);
        CheckSwitchStates();
    }

    public override Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed)
    {
        return projectedPosition + directionToMove * speed * Time.fixedDeltaTime;
    }


}
