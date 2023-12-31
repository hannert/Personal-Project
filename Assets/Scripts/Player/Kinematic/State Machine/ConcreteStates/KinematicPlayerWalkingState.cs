﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KinematicPlayerWalkingState : KinematicPlayerMovementState
{
    public KinematicPlayerWalkingState(KinematicPlayer player, KinematicPlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
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
        InitializeSubState();
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
        // Check if we have a weapon!
        // TODO: Further check for a weapon must be done later!
        if (_psm.isEquipped)
        {
            SetSubState(player.playerIdleWeaponState);
        }
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
        var prevCopy = _psm.projectedPos - _psm.playerRb.position;
        var endDirection = Vector3.zero;

        // Vector3 of proposed player position if player input being taken into account in relation to a camera
        var normalWalkPosition = Vector3.zero;

        var walkingVelocity = Vector3.zero;
        var collideVelocity = Vector3.zero;
        var finalVelocity = Vector3.zero;

        bool goingUpSlope = false;
        bool goingDownSlope = false;

        // Player is on the ground
        if (_psm.onGround)
        {
            var directionOfPlayerForwardRotation = KinematicPlayerUtilities.getDirectionFromOrigin(_psm.playerRb.rotation.eulerAngles.y);
            normalWalkPosition = KinematicPlayerUtilities.GetDirectionFromCamera(_psm.projectedPos, _psm.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            endDirection = normalWalkPosition;
        }


        // Player is NOT on the ground
        if (!_psm.onGround)
        {
            // should be able to control the player a little bit when they are in the air
            // the strength of the rotation end direction should be dampened
            normalWalkPosition = KinematicPlayerUtilities.GetDirectionFromCamera(
                _psm.projectedPos, _psm.projectedPos + _psm.distanceFromCameraAtJump, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            endDirection = Vector3.Lerp(endDirection, normalWalkPosition, 0.2f);
            

        }
        
        if (_psm.onGround) {

            //endDirection = PlayerUtilities.moveOnSlope(_psm.playerRb, _psm.playerCap, endDirection, _psm.projectedPos, _psm.skinWidth);
        }


        #region Wall collision and Movement application
        // Calculate the position the player will move to
        // If the player is not locked on, proceed with movement based on where the camera is looking. (use endDirection)
        // If player IS locked on, move the player based on an axis based on the Vector3 of the LockOnTarget and the Player rather than the camera. (use normalWalkPosition)

        Vector3 directionBuffer = Vector3.zero; // Placeholder zero, it will ALWAYS be changed from the two if statements below 

        // If player is NOT locked on, use the endDirection variable to get the player's velocity vector to PASS into the CollideAndSlide algorithm
        if (!_psm.camera.isLockedOn)
        {
            directionBuffer = endDirection.normalized * (_psm.speed) * Time.fixedDeltaTime;
            directionBuffer = KinematicPlayerUtilities.GetDirectionFromCamera(_psm.playerRb.position, _psm.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        // If player IS locked on, ditto
        if (_psm.camera.isLockedOn)
        {
            directionBuffer = normalWalkPosition.normalized * (_psm.speed) * Time.fixedDeltaTime;
        }

        collideVelocity = KinematicPlayerUtilities.collideAndSlide(_psm.playerCap, directionBuffer, _psm.playerRb.position, 0, _psm.skinWidth, 3, false, directionBuffer, _psm.onGround);

        // Player is on ground, cast forward and DOWN (Going down ramps)
        if (_psm.onGround)
        {
            collideVelocity += KinematicPlayerUtilities.collideAndSlide(_psm.playerCap, _psm.gravityVector, _psm.playerRb.position + (collideVelocity * _psm.speed * Time.fixedDeltaTime), 0, _psm.skinWidth, 3, true, _psm.gravityVector, _psm.onGround);
        } 


        finalVelocity = collideVelocity;
        Debug.Log("Final Velocity:" + finalVelocity);
        _psm.projectedPos = CalculatePositionToMoveTo(_psm.playerRb.position, finalVelocity.normalized, _psm.speed);
        #endregion


        #region Rotate the players model
        // Get rotation of the player to reflect where the player is headed
        rotatePlayer(endDirection);
        #endregion

        // Actually move the player
        if (!_psm.onGround)
        {
            _psm.projectedPos = CalculatePositionToMoveTo(_psm.playerRb.position, finalVelocity.normalized, _psm.speed);


            // If player is ascending
            if(_psm.yVelocity.y > 0)
            {
                var ceilingVel = KinematicPlayerUtilities.collideAndSlide(_psm.playerCap, _psm.yVelocity, _psm.projectedPos, 0, _psm.skinWidth, 3, true, _psm.yVelocity, _psm.onGround);
                var newDirection = ceilingVel.normalized * _psm.yVelocity.magnitude;
                _psm.projectedPos += (newDirection * Time.fixedDeltaTime + ((0.5f) * _psm.gravityVector * Time.fixedDeltaTime * Time.fixedDeltaTime));
            } else
            {
                _psm.projectedPos += (_psm.yVelocity * Time.fixedDeltaTime + ((0.5f) * _psm.gravityVector * Time.fixedDeltaTime * Time.fixedDeltaTime));
            }
            



            
        }

        _psm.playerRb.MovePosition(_psm.projectedPos);
        //stopOnCollision();
        CheckSwitchStates();
    }

    // Method to apply the Velocity vector to the player rigidbody
    public override Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed)
    {
        return projectedPosition + directionToMove * speed * Time.fixedDeltaTime;
    }

    // Debug method to stop the game when player is colliding with a obstacle layer object.
    // We use this because the capsule cast detection does not work if the collider is already within the object we are trying to hit with the capsule collider 
    public void stopOnCollision()
    {
        //We dont need point1 right now, but will be used if we need to use a capsule cast to reflect the players capsule collider size
        var localPoint1 = _psm.playerCap.center - Vector3.down * (_psm.playerCap.height / 2 - (_psm.playerCap.radius - 0.2f));
        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint2 = _psm.playerCap.center + Vector3.down * (_psm.playerCap.height / 2 - (_psm.playerCap.radius - 0.2f)) * 1.1f; // Above point

        var point1 = _psm.playerCap.transform.TransformPoint(localPoint1);
        var point2 = _psm.playerCap.transform.TransformPoint(localPoint2);

        if (Physics.OverlapCapsuleNonAlloc(point1, point2, _psm.playerCap.radius, _psm.wallColliders, LayerMask.GetMask("Wall")) != 0)
        {
            Debug.Break();
        }
    }

    // Method to rotate the player rigidbody with a supplied direction vector
    private void rotatePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // If player is NOT locked on
            if (!_psm.camera.isLockedOn)
            {
                var directionOfMovement = Quaternion.LookRotation(direction, Vector3.up);
                directionOfMovement = Quaternion.Euler(0, directionOfMovement.eulerAngles.y, 0);
                _psm.playerRb.MoveRotation(directionOfMovement);
            }
            else
            {
                // If locked on, look at the object that is locked onto
                // Currently, we only want the Y rotation component from the LookRotation. 
                // !!! TODO: Perhaps we want all of the rotation for in-air movement and further states
                var directionToLockOn = Quaternion.LookRotation(_psm.camera.lockOnFocusObject.transform.position - _psm.playerRb.position, Vector3.up);
                directionToLockOn = Quaternion.Euler(0, directionToLockOn.eulerAngles.y, 0);
                _psm.playerRb.MoveRotation(directionToLockOn);



            }
        }

        
    }
}
