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

    // Variable to keep track of the first time the user enters, use impulse to push to make it feel more snappy
    bool initPush = false;

    Vector3 directionOfRoll;

    public PlayerRollingState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
    }

    public override void AddForceToRB(Vector3 acceleration)
    {
        if(_psm.onGround == true)
        {
            _psm.playerRb.AddForce(acceleration * Time.fixedDeltaTime * 150f, ForceMode.Force);
        }
        if(_psm.onGround == false)
        {
            _psm.playerRb.AddForce(acceleration * Time.fixedDeltaTime * 1.5f, ForceMode.Force);
        }
    }

   



    public override bool CheckSwitchStates()
    {
        if (distanceRolled > distanceToRoll)
        {
            Debug.Log("LET ME OUT!");
            if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
            {
                SwitchState(player.playerIdleState);
                return true;
            } else
            {
                if (_psm.horizontalInput != 0 || _psm.verticalInput != 0)
                {

                    // Left shift detected, go into sprint
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SwitchState(player.playerSprintingState);
                        return true;
                    }

                    // Else go into normal walking
                    else
                    {
                        SwitchState(player.playerWalkingState);
                        return true;
                    }

                }
            }

        }
        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Enter Roll");
        initPush = true;
        _psm.isRolling = true;
        _psm.playerAnim.SetBool("isRolling", true);

        // Reset our counter variables
        distanceRolled = 0;
        timeSpentRolling = 0;
        distanceToRoll = _psm.speed * 0.8f;

        // Once we enter, we need the direction of the roll!
        if (_psm.isLockedOn)
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_psm.camera.lockOnFocusObject.transform.position, _psm.playerRb.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            directionOfRoll = PlayerUtilities.GetDirectionFromCamera(_psm.playerRb.position, _psm.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        

        Debug.Log(distanceToRoll);
    }

    public override void ExitState()
    {
        _psm.isRolling = false;
        _psm.playerAnim.SetBool("isRolling", false);

        Debug.Log("Exit Roll");
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
        var distanceToMove = _psm.speed * Time.fixedDeltaTime;
        distanceRolled += distanceToMove;
        Debug.Log(distanceRolled);
        // Add constant force while turn on ghosting for interactable entities?

        // Initial press should impulse the player torwards the direction
        if(initPush) {
            float multiplier = 200f;
            if (!_psm.onGround)
            {
                multiplier = 100f;
            }
            _psm.playerRb.AddForce(directionOfRoll * _psm.speed * Time.fixedDeltaTime * multiplier, ForceMode.Impulse);
            initPush = false;
        } 
        else
        {
            _psm.playerRb.AddForce(directionOfRoll * _psm.speed * Time.fixedDeltaTime, ForceMode.Force);
        }
        

        if (CheckSwitchStates()) return;
    }

}
