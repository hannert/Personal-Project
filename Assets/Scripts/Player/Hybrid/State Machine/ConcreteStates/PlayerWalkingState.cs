using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Player state where the player is moving
/// </summary>
public class PlayerWalkingState : PlayerMovementState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name) { }

    public override bool CheckSwitchStates()
    {
        if (Input.GetKey(Keybinds.sprint))
        {
            // If player is crouching and tries to sprint, eat the input
            if(_psm.isCrouched) { return false;  }
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
        //_psm.playerAnim.SetBool("isWalking", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Walking state");
        _psm.isWalking = false;
        //_psm.playerAnim.SetBool("isWalking", false);
    }

    public override void FrameUpdate()
    {
        if (Input.GetKeyDown(Keybinds.roll))
        {

            SwitchState(player.playerRollingState);
        }
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_psm.canJump == true)
            {
                _psm.willJump = true;
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
        // crouch state as an intermediate state between root and movement states

        #region  Get direction of player input

        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(_psm.playerRb.position.x, 0, _psm.playerRb.position.z);
        var tempCamera = new Vector3(_psm.camera.transform.position.x, 0, _psm.camera.transform.position.z);

        if (_psm.isLockedOn)
        {
            // the locked on object will become the 'player'
            tempPlayer = new Vector3(_psm.camera.lockOnFocusObject.transform.position.x, 0, _psm.camera.lockOnFocusObject.transform.position.z);
            tempCamera = new Vector3(_psm.playerRb.position.x, 0, _psm.playerRb.position.z);
        }


        // Get the direction from the object (camera) to the reference (player)
        var objectToReferenceDirection = (tempPlayer - tempCamera).normalized;


        // Get horizontal and vertical components from input ------

        // Already normalized ( 0 - 1 value for x and z )
        // Use horizontal input, 'a' or 'd' key
        var horizontalVector = new Vector3(_psm.horizontalInput, 0, 0);
       
        // Use vertical input, 'w' or 's' key
        var verticalVector = new Vector3(0, 0, _psm.verticalInput);

        // Currently overriding the horizontal component with the Axis Raw as our own input smoother still needs work (_psm.iput)
        // TODO: Change back to custom input
        horizontalVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        verticalVector = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));

        // ------


        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(objectToReferenceDirection.z, 0, -objectToReferenceDirection.x);

        // The direction the player's key input is pointing towards
        var endDirection = betweenVector.x * perpVector + betweenVector.z * objectToReferenceDirection;


        #endregion

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            var finalRotation = testFinalRotation.eulerAngles.normalized;
            //playerRb.rotation = testFinalRotation;
        }

        Vector3 currentPlayerVelocityDirection = _psm.playerRb.velocity.normalized;

        // Get the Vector3 dot product from the players current velocity and the desired velocity direction (endDirection)
        // The dot product will tell us how similar the new direction is to the current players velocity 
        // From the Unity Documentation ->
        // For normalized vectors Dot returns 1 if they point in exactly the same direction, -1 if they point in completely opposite directions and zero if the vectors are perpendicular.
        float velDotProduct = Vector3.Dot(endDirection, currentPlayerVelocityDirection);

        // From the dot product, we can get a custom value if the player is trying to switch directions, where the acceleration will be about 2x stronger to account for the 
        float accel = _psm.acceleration * _psm.AccelerationMultiplier.Evaluate(velDotProduct);

        Vector3 FinalVel = _psm.playerRb.velocity;

        Vector3 goalVelocity = endDirection * _psm.speed;

        FinalVel = Vector3.MoveTowards(FinalVel, goalVelocity + _psm.playerRb.velocity, accel * Time.fixedDeltaTime);

        //Debug.Log(accel);

        Vector3 neededAcceleration = (FinalVel - _psm.playerRb.velocity) / Time.fixedDeltaTime;

        neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, _psm.maxAcceleration);


        if (!_psm.onGround)
        {
            // While in the air, input should influence the player's movement less, by a multiplier
            AddForceToRB(neededAcceleration * 0.4f);
        }
        else
        {
            // Else not in the air (On ground), player should move normally
            AddForceToRB(neededAcceleration);
        }


        // We need to cast it from inside the player to avoid the issue of the raycast already intersecting the stair in question, thereby not firing the raycast at all
        if (_psm.onGround)
        {
            PlayerUtilities.stepCast(
                _psm.playerRb, 
                _psm.playerRb.transform.position + new Vector3(0f, -0.8f, 0f), 
                _psm.playerRb.transform.position + new Vector3(0f, -0.6f, 0f), 
                endDirection);
        }
        

        #region Rotate the players model
        // Get rotation of the player to reflect where the player is headed
        rotatePlayer(endDirection);
        #endregion



        CheckSwitchStates();
    }

    // Method to apply the Velocity vector to the player rigidbody
    // Seperated into another method so that child states can use this to change speed
    public override void AddForceToRB(Vector3 acceleration)
    {
        _psm.playerRb.AddForce(Vector3.Scale(acceleration * _psm.playerRb.mass * _psm.speedMultiplier, new Vector3(1, 0, 1)));
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
