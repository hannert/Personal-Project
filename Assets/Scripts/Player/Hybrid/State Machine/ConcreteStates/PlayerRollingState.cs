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

    Vector3 directionOfRoll;

    public PlayerRollingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AddForceToRB(Vector3 directionToMove, float speed)
    {
        throw new System.NotImplementedException();
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

        _psm.isRolling = true;
        _psm.playerAnim.SetBool("isRolling", true);

        // Reset our counter variables
        distanceRolled = 0;
        timeSpentRolling = 0;
        distanceToRoll = _psm.speed * 0.8f;

        // Once we enter, we need the direction of the roll!
        if (_psm.isLockedOn)
        {
            directionOfRoll = KinematicPlayerUtilities.GetDirectionFromCamera(_psm.camera.lockOnFocusObject.transform.position, _psm.playerRb.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            directionOfRoll = KinematicPlayerUtilities.GetDirectionFromCamera(_psm.projectedPos, _psm.camera.transform.position, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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


        if (CheckSwitchStates()) return;
    }

}