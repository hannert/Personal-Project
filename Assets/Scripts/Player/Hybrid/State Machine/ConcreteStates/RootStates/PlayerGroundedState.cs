using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    bool goingToCrouch = false;
    bool goingToSlide = false;
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered grounded state");
        //_psm.playerAnim.SetBool("isGrounded", true);
        _psm.onGround = true;
        _psm.canJump = true;
        _psm.playerRb.drag = 5;
        goingToSlide = false;
        goingToCrouch = false;
        InitializeSubState();
        
    }

    public override void ExitState()
    {
        Debug.Log("Exited grounded root state");
        _psm.onGround = false;
        //_psm.playerAnim.SetBool("isGrounded", false);
        _psm.playerRb.drag = 1;

    }

    public override void FrameUpdate()
    {
        // If player is not crouched
        if (!_psm.isCrouched)
        {
            // If player is not sprinting
            if (!_psm.isSprinting)
            {
                // IF player is not crouched and not sprinting, press crouch to initate crouch on next physics update cycle
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    goingToCrouch = true;
                    return;
                }
            }
            // If player is sprinting
            if (_psm.isSprinting)
            {
                // If player is not crouched and sprinting, get ready to slide
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _psm.isSliding = true;
                    goingToSlide = true;
                    return;
                }
            }
        }
    }

    public override void InitializeSubState()
    {
        // If player holds down crouch, enter crouch
        if (!_psm.isCrouched)
        {
            if (!_psm.isSprinting)
            {
                if (Input.GetKeyDown(Keybinds.crouch))
                {
                    Debug.Log("I want to crouch!");
                    goingToCrouch = true;
                    return;
                }
            }
            if (_psm.isSprinting)
            {
                if (Input.GetKeyDown(KeyCode.Mouse3))
                {
                    _psm.isSliding = true;
                    goingToSlide = true;
                    return;

                }
            }
        }

        // While grounded, not moving -> Idle
        if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            SetSubState(player.playerIdleState);
        } else
        {
            SetSubState(player.playerWalkingState);
        }
        

    }

    public override void PhysicsUpdate()
    {
        if (goingToSlide == true)
        {
            _currentSubState.ExitState();
            SetSubState(player.playerCrouchState);
            goingToSlide = false;
            goingToCrouch = false;
            return;
        }

        if (goingToCrouch == true)
        {
            _currentSubState.ExitState();
            SetSubState(player.playerCrouchState);
            goingToCrouch = false;
            return;
        }

        if (_psm.checkGround == true || _psm.playerRb.velocity.y <= 0)
        {
            // When grounded, check if there is floor beneath to trigger falling state
            if (PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) == 0)
            {
                _psm.onGround = false;
            }
        }
        
        CheckSwitchStates();
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.willJump)
        {
            _psm.playerRb.AddForce(Vector3.up * _psm.jumpForce, ForceMode.Impulse);
            _psm.canJump = false;
            _psm.checkGround = false;
            _psm.willJump = false;
            SwitchState(player.playerAirState);
            return true;

        }
        if (_psm.onGround == false)
        {
            SwitchState(player.playerAirState);
            return true;
        }

        return false;

    }



}
