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
        _ctx.onGround = true;
        _ctx.canJump = true;
        _ctx.playerRb.drag = 5;
        _ctx.extraJumpsTaken = 0; // Reset extra jumps taken
        _ctx.regJumpTaken = false; // reset regular jump
        goingToSlide = false;
        goingToCrouch = false;
        InitializeSubState();

    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=brown>grounded</color> State");
        _ctx.onGround = false;
        _ctx.playerRb.drag = 1;

    }

    public override void FrameUpdate()
    {
        // If player is not crouched
        if (!_ctx.isCrouched)
        {
            // If player is not sprinting
            if (!_ctx.isSprinting)
            {
                // IF player is not crouched and not sprinting, press crouch to initate crouch on next physics update cycle
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    goingToCrouch = true;
                    return;
                }
            }
            // If player is sprinting
            if (_ctx.isSprinting || _ctx.isWalking)
            {
                // If player is not crouched and sprinting, get ready to slide
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _ctx.isSliding = true;
                    goingToSlide = true;
                    return;
                }
            }
        }
    }

    public override void InitializeSubState()
    {
        // If player holds down crouch, enter crouch
        if (!_ctx.isCrouched)
        {
            if (!_ctx.isSprinting)
            {
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    Debug.Log("I want to crouch!");
                    goingToCrouch = true;
                    return;
                }
            }
            if (_ctx.isSprinting || _ctx.isWalking)
            {
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _ctx.isSliding = true;
                    goingToSlide = true;
                    return;

                }
            }
        }

        // While grounded, not moving -> Idle
        if (_ctx.horizontalInput == 0 && _ctx.verticalInput == 0)
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
        if (_ctx.willJump && !_ctx.regJumpTaken)
        {
            _ctx.playerRb.AddForce(Vector3.up * _ctx.jumpForce, ForceMode.Impulse);

            // Consume the regular jump 
            _ctx.regJumpTaken = true;
            //_psm.canJump = false;
            //_psm.checkGround = false;
            _ctx.willJump = false;
            SwitchState(_factory.Airborne());
            return true;

        }
        if (_ctx.onGround == false)
        {
            SwitchState(_factory.Airborne());
            return true;
        }

        return false;

    }



}
