using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWalkingState : PlayerMovementState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
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
        Debug.Log("Exited Walking state");
        _psm.isWalking = false;
        _psm.playerAnim.SetBool("isWalking", false);
    }

    public override void FrameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {

            SwitchState(player.playerRollingState);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_psm.canJump == true)
            {
                _psm.canJump = false;
                Debug.Log("Jumped");
                _psm.playerRb.AddForce(Vector3.up * _psm.jumpForce, ForceMode.Impulse);
            }
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


        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(_psm.playerRb.position.x, 0, _psm.playerRb.position.z);
        var tempCamera = new Vector3(_psm.camera.transform.position.x, 0, _psm.camera.transform.position.z);

        if (_psm.isLockedOn)
        {
            // the locked on object will become the 'player'
            tempPlayer = new Vector3(_psm.camera.lockOnFocusObject.transform.position.x, 0, _psm.camera.lockOnFocusObject.transform.position.z);
            tempCamera = new Vector3(_psm.playerRb.position.x, 0, _psm.playerRb.position.z);
        }


        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(_psm.horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, _psm.verticalInput);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;


        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            var finalRotation = testFinalRotation.eulerAngles.normalized;
            //playerRb.rotation = testFinalRotation;
        }
        if (!_psm.onGround && _psm.playerRb.velocity.y < 0)
        {
            _psm.playerRb.AddForce(Physics.gravity * 3, ForceMode.Acceleration);
        }

        if (!_psm.onGround)
        {
            // While in the air, input should influence the player's movement less, by a multiplier
            AddForceToRB(endDirection.normalized, _psm.speed * 0.4f);
        }
        else
        {
            AddForceToRB(endDirection.normalized, _psm.speed);
        }
        

        #region Rotate the players model
        // Get rotation of the player to reflect where the player is headed
        rotatePlayer(endDirection);
        #endregion



        CheckSwitchStates();
    }

    // Method to apply the Velocity vector to the player rigidbody
    public override void AddForceToRB(Vector3 directionToMove, float speed)
    {
        _psm.playerRb.AddForce(directionToMove.normalized * speed * Time.fixedDeltaTime * 4, ForceMode.VelocityChange);
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
