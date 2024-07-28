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
    public PlayerWalkingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name) { }

    public override bool CheckSwitchStates()
    {
        if (Input.GetKey(Keybinds.sprint))
        {
            // If player is crouching and tries to sprint, eat the input
            if(_ctx.isCrouched) { return false;  }
            SwitchState(_factory.Sprinting());
            return true;

        }

        if (_ctx.horizontalInput == 0 && _ctx.verticalInput == 0)
        {
            SwitchState(_factory.Idle());
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=olive>Walking</color> State");
        _ctx.isWalking = true;
        InitializeSubState();
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=olive>Walking</color> State");
        _ctx.isWalking = false;
    }

    public override void FrameUpdate()
    {
        //if (Input.GetKey(Keybinds.slide) ||Input.GetKey(Keybinds.slideAlt))
        //{
        //    SwitchState(player.playerSlidingState);
        //    return;
        //}

        if (Input.GetKeyDown(Keybinds.roll))
        {

            SwitchState(_factory.Rolling());
        }
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_ctx.canJump == true)
            {
                Debug.Log("WillJump set to true");
                _ctx.willJump = true;
            }
        }
    }

    public override void InitializeSubState()
    {
        // Check if we have a weapon!
        // TODO: Further check for a weapon must be done later!
        // if (_ctx.isEquipped)
        // {
        //     SetSubState(player.playerIdleWeaponState);
        // }
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


        // The direction the player's key input is pointing towards
        var endDirection = PlayerUtilities.GetInputDirection(_ctx);


        #endregion

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            var finalRotation = testFinalRotation.eulerAngles.normalized;
            //playerRb.rotation = testFinalRotation;
        }

        Vector3 currentPlayerVelocityDirection = _ctx.playerRb.velocity.normalized;

        // Get the Vector3 dot product from the players current velocity and the desired velocity direction (endDirection)
        // The dot product will tell us how similar the new direction is to the current players velocity 
        // From the Unity Documentation ->
        // For normalized vectors Dot returns 1 if they point in exactly the same direction, -1 if they point in completely opposite directions and zero if the vectors are perpendicular.
        float velDotProduct = Vector3.Dot(endDirection, currentPlayerVelocityDirection);

        // From the dot product, we can get a custom value if the player is trying to switch directions, where the acceleration will be about 2x stronger to account for the 
        float accel = _ctx.acceleration * _ctx.AccelerationMultiplier.Evaluate(velDotProduct);

        Vector3 FinalVel = _ctx.playerRb.velocity;
        endDirection = PlayerUtilities.CollideAndSlide(_ctx.playerCap, endDirection, _ctx.playerRb.position, 0, 0.05f, 3, true, _ctx.playerRb.velocity, _ctx.onGround);
        Vector3 goalVelocity = endDirection * _ctx.speed;
        // Debug.Log(goalVelocity);

        FinalVel = Vector3.MoveTowards(FinalVel, goalVelocity + _ctx.playerRb.velocity, accel * Time.fixedDeltaTime);
        //Debug.Log(accel);

        Vector3 neededAcceleration = (FinalVel - _ctx.playerRb.velocity) / Time.fixedDeltaTime;

        neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, _ctx.maxAcceleration);


        if (!_ctx.onGround)
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
        if (_ctx.onGround)
        {
            PlayerUtilities.stepCast(
                _ctx.playerRb, 
                _ctx.playerRb.transform.position + new Vector3(0f, -0.8f, 0f), 
                _ctx.playerRb.transform.position + new Vector3(0f, -0.6f, 0f), 
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
        Vector3 addVel = Vector3.Scale(acceleration * _ctx.playerRb.mass * _ctx.speedMultiplier, new Vector3(1, 0, 1));
        //Vector3 FinalVel = PlayerUtilities.CollideAndSlide(_ctx.playerCap, Vector3.Scale(acceleration,_ctx.playerRb.velocity), _ctx.playerRb.position, 0, 0.05f, 3, true, _ctx.playerRb.velocity, _ctx.onGround);

        // Move and slide
        _ctx.playerRb.AddForce(addVel);
    }


    // Method to rotate the player rigidbody with a supplied direction vector
    private void rotatePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // If player is NOT locked on
            if (!_ctx.camera.isLockedOn)
            {
                var directionOfMovement = Quaternion.LookRotation(direction, Vector3.up);
                directionOfMovement = Quaternion.Euler(0, directionOfMovement.eulerAngles.y, 0);
                _ctx.playerRb.MoveRotation(directionOfMovement);
            }
            else
            {
                // If locked on, look at the object that is locked onto
                // Currently, we only want the Y rotation component from the LookRotation. 
                // !!! TODO: Perhaps we want all of the rotation for in-air movement and further states
                var directionToLockOn = Quaternion.LookRotation(_ctx.camera.lockOnFocusObject.transform.position - _ctx.playerRb.position, Vector3.up);
                directionToLockOn = Quaternion.Euler(0, directionToLockOn.eulerAngles.y, 0);
                _ctx.playerRb.MoveRotation(directionToLockOn);



            }
        }

        
    }
}
