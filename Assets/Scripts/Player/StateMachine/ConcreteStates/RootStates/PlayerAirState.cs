using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    // Timer for coyote time allowable jumps
    float currentTimer = 0f;
    float maxCoyoteTime = 0.16f;

    public PlayerAirState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
        _isRootState = true;
    }

    public override bool CheckSwitchStates()
    {
        if (_ctx.IsFalling == true)
        {
            if (PlayerUtilities.CheckGroundCollision(_ctx.GroundColliders, _ctx.PlayerCap) != 0)
            {
                Logging.logState("Ground collision returned true in air state");
                SwitchState(_factory.Grounded());
                return true;
            }
        }
        return false;
    }

    public override void EnterState()
    {

        Logging.logState("<color=green>Entered</color> <color=lightblue>Air</color> State");

        _ctx.OnGround = false;

        // If number of jumps taken exceeds max, player can no longer jump
        _ctx.CanJump = _ctx.ExtraJumpsTaken < _ctx.ExtraJumpsMax;

        //_ctx.RegJumpTaken = false;
        InitializeSubState();        
    }

    public override void ExitState()
    {

        Logging.logState("<color=red>Exited</color> <color=lightblue>Air</color> State");

        // Normally exit to the Grounded State
        _ctx.IsJumping = false;
        _ctx.IsFalling = false;
        currentTimer = 0f;
    }

    public override void FrameUpdate()
    {
        // Check for player input 
        // Check if jump key is pressed
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_ctx.CanJump == true)
            {
                _ctx.WillJump = true;
                Logging.logState("Will jump in next physics update cycle from <color=lightblue>AirState</color>");
                
            }
        }

        // Get input whether or not player should slide when hitting the ground!

        
    }

    public override void InitializeSubState()
    {
        if (_ctx.IsAttacking) 
        {
            Debug.Log("Is Attacking true");
            return;
        }
        if (_ctx.HorizontalInput == 0 && _ctx.VerticalInput == 0)
        {
            SetSubState(_factory.Idle());
        }
        else
        {
            SetSubState(_factory.Walking());
        }

    }

    public override void PhysicsUpdate()
    {
        
       
        // Coyote time will only account for the original singular jump. Shouldn't consume an [Extra] jump.
        if (!_ctx.RegJumpTaken && currentTimer <= maxCoyoteTime)
        {
            // Add to the coyote time timer
            currentTimer += Time.fixedDeltaTime;

            // Execute code when timer runs out naturally
            if(currentTimer >= maxCoyoteTime)
            {
                // Consume regular jump
                _ctx.RegJumpTaken = true;
                Logging.logState("Timer Ran out");
            }

            if (_ctx.WillJump)
            {
                Logging.logState("Jump Regular");
                // If regular jump is not taken yet, consume it 

                // What if we stopped the player's velocity at this instant?
                Vector3 temp = _ctx.PlayerRb.velocity;
                temp.y = 0;
                _ctx.PlayerRb.velocity = temp;

                _ctx.PlayerRb.AddForce(Vector3.up * _ctx.JumpForce, ForceMode.Impulse);
                _ctx.RegJumpTaken = true;
                _ctx.CanJump = true;
                _ctx.WillJump = false;
            }
        }
        // Coyote timer ran out, account for the extra jumps now.
        else
        {
            if (_ctx.WillJump)
            {
                // Increment the number of extra jumps taken
                _ctx.ExtraJumpsTaken += 1;

                // If the player is still within the extra jump limit
                if (_ctx.ExtraJumpsTaken <= _ctx.ExtraJumpsMax) {
                    Logging.logState("Jump Extra");

                    // Apply upward force
                    Vector3 temp = _ctx.PlayerRb.velocity;
                    temp.y = 0;
                    _ctx.PlayerRb.velocity = temp;

                    _ctx.PlayerRb.AddForce(Vector3.up * _ctx.JumpForce, ForceMode.Impulse);
                }
                // If number of jumps taken exceeds max, player can no longer jump
                _ctx.CanJump = _ctx.ExtraJumpsTaken < _ctx.ExtraJumpsMax;

                _ctx.WillJump = false;
            }
        }
        


        _ctx.PlayerRb.AddForce(Physics.gravity * 3, ForceMode.Acceleration);

        // Update if the player bool for is the player falling or not ( negative y velocity )
        if (_ctx.PlayerRb.velocity.y < 0)
        {
            _ctx.IsFalling = true;

            // If player is FALLING, we check for overlapping with walls to SLOW down the descent

            // Add to the radius of the playerCapsule to enable a outer "SKIN" 
            var localPoint1 = _ctx.PlayerCap.center - Vector3.down * (_ctx.PlayerCap.height / 2 - (_ctx.PlayerCap.radius));
            var localPoint2 = _ctx.PlayerCap.center + Vector3.down * (_ctx.PlayerCap.height / 2 - (_ctx.PlayerCap.radius));

            var point1 = _ctx.PlayerCap.transform.TransformPoint(localPoint1);
            var point2 = _ctx.PlayerCap.transform.TransformPoint(localPoint2);

            // Check if wall collision is hitting a wall and store data in wallColliders array in PSM
            Physics.OverlapCapsuleNonAlloc(point1, point2, _ctx.PlayerCap.radius + 0.4f, _ctx.WallColliders, LayerMask.GetMask("Wall"));

            _ctx.IsWallSliding = _ctx.WallColliders[0] != null ? true : false;

            Array.Clear(_ctx.WallColliders, 0, _ctx.WallColliders.Length);


        }      
        
        // If player is falling near a wall, slow down 
        if (_ctx.IsWallSliding && _ctx.IsFalling)
        {
            _ctx.PlayerRb.AddForce(Physics.gravity * -1, ForceMode.Acceleration);
        }


        if (CheckSwitchStates()) return;

    }


}
