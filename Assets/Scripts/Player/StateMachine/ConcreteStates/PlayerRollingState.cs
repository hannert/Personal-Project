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
        if(_ctx.OnGround == true)
        {
            _ctx.PlayerRb.AddForce(acceleration * Time.fixedDeltaTime * 150f, ForceMode.Force);
        }
        if(_ctx.OnGround == false)
        {
            _ctx.PlayerRb.AddForce(acceleration * Time.fixedDeltaTime * 1.5f, ForceMode.Force);
        }
    }

   



    public override bool CheckSwitchStates()
    {
        // Check if the player rolled enough distance
        if (distanceRolled > distanceToRoll)
        {
            // If no input, set player to idle
            if (_ctx.HorizontalInput == 0 && _ctx.VerticalInput == 0)
            {
                SwitchState(_factory.Idle());
                return true;
            }
            // If there is input, go into either sprint or walking
            else
            {
                if (_ctx.HorizontalInput != 0 || _ctx.VerticalInput != 0)
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
        _ctx.IsRolling = true;
        //_ctx.playerAnim.SetBool("isRolling", true);

        // Reset our counter variables
        distanceRolled = 0;
        timeSpentRolling = 0;
        distanceToRoll = _ctx.Speed * 0.8f;

        // Once we enter, we need the direction of the roll!
        if (_ctx.IsLockedOn)
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_ctx.Camera.lockOnFocusObject.transform.position, _ctx.PlayerRb.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_ctx.PlayerRb.position, _ctx.Camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        

    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=silver>Roll</color> State");
        _ctx.IsRolling = false;
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
        var distanceToMove = _ctx.Speed * Time.fixedDeltaTime;
        distanceRolled += distanceToMove;
        // Add constant force while turn on ghosting for interactable entities?

        // Initial press should impulse the player torwards the direction
        if(initPush) {
            float multiplier = 200f;
            if (!_ctx.OnGround)
            {
                multiplier = 100f;
            }
            _ctx.PlayerRb.AddForce(directionOfRoll * _ctx.Speed * Time.fixedDeltaTime * multiplier, ForceMode.Impulse);
            initPush = false;
        } 
        else
        {
            _ctx.PlayerRb.AddForce(directionOfRoll * _ctx.Speed * Time.fixedDeltaTime, ForceMode.Force);
        }
        

        if (CheckSwitchStates()) return;
    }

}
