using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    bool goingToCrouch = false;
    bool goingToSlide = false;
    public PlayerGroundedState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=brown>grounded</color> State");
        _ctx.OnGround = true;
        _ctx.CanJump = true;
        _ctx.PlayerRb.drag = 5;
        _ctx.ExtraJumpsTaken = 0; // Reset extra jumps taken
        _ctx.RegJumpTaken = false; // reset regular jump
        goingToSlide = false;
        goingToCrouch = false;
        InitializeSubState();

    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=brown>grounded</color> State");
        _ctx.OnGround = false;
        _ctx.PlayerRb.drag = 1;

    }

    public override void FrameUpdate()
    {
        if (Input.GetKeyDown(Keybinds.jump))
        {
            if (_ctx.CanJump == true)
            {
                _ctx.WillJump = true;
                Logging.logState("Will jump in next physics update cycle from <color=brown>GroundedState</color>");
                
            }
        }
        // If player is not crouched
        if (!_ctx.IsCrouched)
        {
            // If player is not sprinting
            if (!_ctx.IsSprinting)
            {
                // IF player is not crouched and not sprinting, press crouch to initate crouch on next physics update cycle
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    goingToCrouch = true;
                    return;
                }
            }
            // If player is sprinting
            if (_ctx.IsSprinting || _ctx.IsWalking)
            {
                // If player is not crouched and sprinting, get ready to slide
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _ctx.IsSliding = true;
                    goingToSlide = true;
                    return;
                }
            }
        }
    }

    public override void InitializeSubState()
    {
        // If player holds down crouch, enter crouch
        if (!_ctx.IsCrouched)
        {
            if (!_ctx.IsSprinting)
            {
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    Debug.Log("I want to crouch!");
                    goingToCrouch = true;
                    return;
                }
            }
            if (_ctx.IsSprinting || _ctx.IsWalking)
            {
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _ctx.IsSliding = true;
                    goingToSlide = true;
                    return;

                }
            }
        }

        // While grounded, not moving -> Idle
        if (_ctx.HorizontalInput == 0 && _ctx.VerticalInput == 0)
        {
            SetSubState(_factory.Idle());
        } else
        {
            SetSubState(_factory.Walking());
        }
        

    }

    public override void PhysicsUpdate()
    {
        // if (goingToSlide == true)
        // {
        //     _currentSubState.ExitState();
        //     SetSubState(player.playerCrouchState);
        //     goingToSlide = false;
        //     goingToCrouch = false;
        //     return;
        // }

        // if (goingToCrouch == true)
        // {
        //     _currentSubState.ExitState();
        //     SetSubState(player.playerCrouchState);
        //     goingToCrouch = false;
        //     return;
        // }



        CheckSwitchStates();
    }

    public override bool CheckSwitchStates()
    {

        // ! TODO: The problem of the bunny hop after pressing jump when falling and after Coyote time comes from this line of code!
        if (_ctx.WillJump && !_ctx.RegJumpTaken)
        {
            _ctx.PlayerRb.AddForce(Vector3.up * _ctx.JumpForce, ForceMode.Impulse);

            // Consume the regular jump 
            _ctx.RegJumpTaken = true;
            //_psm.canJump = false;
            //_psm.checkGround = false;
            _ctx.WillJump = false;
            SwitchState(_factory.Airborne());
            return true;

        }

        // Check if player is in the air 
        if (PlayerUtilities.CheckGroundCollision(_ctx.GroundColliders, _ctx.PlayerCap) == 0){
            SwitchState(_factory.Airborne());
            return true;
        }


        return false;

    }



}
