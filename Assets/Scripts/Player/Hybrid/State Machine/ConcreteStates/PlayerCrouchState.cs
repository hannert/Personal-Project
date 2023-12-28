using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    private float startYScale;

    public PlayerCrouchState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    { 
        startYScale = _psm.playerRb.transform.localScale.y; 
    }



    public override bool CheckSwitchStates()
    {

        // Leave this state. Would be from grounded
        // Set the parent's state child state to this state's child
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Debug.Log("WAOW!");
            if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
            {
                SwitchState(player.playerIdleState);
                return true;
            }
            else
            {
                SwitchState(player.playerWalkingState);
                return true;                            
            }
        }




        return false;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Crouch state");
        _psm.isCrouched = true;
        _psm.playerRb.transform.localScale = new Vector3(_psm.playerRb.transform.localScale.x, 0.5f, _psm.playerRb.transform.localScale.z);

        // We'll also have to move the player downward
        _psm.playerRb.MovePosition(new Vector3(_psm.playerRb.position.x, _psm.playerRb.position.y - 0.5f, _psm.playerRb.position.z));


        InitializeSubState();
        _psm.checkGround = true;
    }

    public override void ExitState()
    {
        Debug.Log("Exited Crouch state");
        _psm.isCrouched = false;
        _psm.playerRb.transform.localScale = new Vector3(_psm.playerRb.transform.localScale.x, startYScale, _psm.playerRb.transform.localScale.z);

        
    }

    public override void FrameUpdate()
    {
        CheckSwitchStates();
    }

    public override void InitializeSubState()
    {
        // While grounded and crouched, not moving -> Idle
        if (_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            SetSubState(player.playerIdleState);
        }
        else
        {
            SetSubState(player.playerWalkingState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
