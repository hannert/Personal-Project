using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerRollingState : PlayerMovementState
{

    float distanceRolled;
    float distanceToRoll;
    float timeSpentRolling;
    float timeToRoll;

    /// <summary>
    /// Variable to keep track of the first time the user enters, use impulse to push to make it feel more snappy
    /// </summary>
    bool initPush = false;

    Vector3 directionOfRoll;

    public PlayerRollingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }

    public override void AddForceToRB(Vector3 acceleration)
    {
        if(_ctx.onGround == true)
        {
            _ctx.playerRb.AddForce(acceleration * Time.fixedDeltaTime * 150f, ForceMode.Force);
        }
        if(_ctx.onGround == false)
        {
            _ctx.playerRb.AddForce(acceleration * Time.fixedDeltaTime * 1.5f, ForceMode.Force);
        }
    }

   



    public override bool CheckSwitchStates()
    {
        // Check if the player rolled enough distance
        if (distanceRolled > distanceToRoll)
        {
            // If no input, set player to idle
            if (_ctx.horizontalInput == 0 && _ctx.verticalInput == 0)
            {
                SwitchState(_factory.Idle());
                return true;
            }
            // If there is input, go into either sprint or walking
            else
            {
                if (_ctx.horizontalInput != 0 || _ctx.verticalInput != 0)
                {

                    // Left shift detected, go into sprint
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SwitchState(_factory.Sprinting());
                        return true;
                    }

                    // Else go into normal walking
                    else
                    {
                        SwitchState(_factory.Walking());
                        return true;
                    }

                }
            }

        }
        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=silver>Roll</color> State");

        initPush = true;
        _ctx.isRolling = true;
        //_ctx.playerAnim.SetBool("isRolling", true);

        // Reset our counter variables
        distanceRolled = 0;
        timeSpentRolling = 0;
        distanceToRoll = _ctx.speed * 0.8f;

        // Once we enter, we need the direction of the roll!
        if (_ctx.isLockedOn)
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_ctx.camera.lockOnFocusObject.transform.position, _ctx.playerRb.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_ctx.playerRb.position, _ctx.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        

    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=silver>Roll</color> State");
        _ctx.isRolling = false;
        //_ctx.playerAnim.SetBool("isRolling", false);
        
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // If roll interuptable: get input here
    }

    public override void InitializeSubState()
    {
        base.InitializeSubState();
    }

    public override void PhysicsUpdate()
    {
        var distanceToMove = _ctx.speed * Time.fixedDeltaTime;
        distanceRolled += distanceToMove;
        // Add constant force while turn on ghosting for interactable entities?

        // Initial press should impulse the player torwards the direction
        if(initPush) {
            float multiplier = 200f;
            if (!_ctx.onGround)
            {
                multiplier = 100f;
            }
            _ctx.playerRb.AddForce(directionOfRoll * _ctx.speed * Time.fixedDeltaTime * multiplier, ForceMode.Impulse);
            initPush = false;
        } 
        else
        {
            _ctx.playerRb.AddForce(directionOfRoll * _ctx.speed * Time.fixedDeltaTime, ForceMode.Force);
        }
        

        if (CheckSwitchStates()) return;
    }

}
