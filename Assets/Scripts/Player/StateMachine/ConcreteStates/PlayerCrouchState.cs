using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    private float startYScale;

    private bool slideFlag = false;

    public PlayerCrouchState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    { 
        startYScale = _ctx.PlayerRb.transform.localScale.y; 
    }



    public override bool CheckSwitchStates()
    {

        // Leave this state. Would be from grounded
        // Set the parent's state child state to this state's child
        if (Input.GetKeyUp(Keybinds.crouch))
        {
            Logging.logState("Let go of <color=red>crouch</red>!");
            if (_ctx.HorizontalInput == 0 && _ctx.VerticalInput == 0)
            {
                SwitchState(_factory.Idle());
                return true;
            }
            else
            {
                SwitchState(_factory.Walking());
                return true;                            
            }
        }




        return false;
    }

    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=blue>Crouch</color> State");
        _ctx.IsCrouched = true;
        //_ctx.checkGround = true;
        _ctx.SpeedMultiplier = 0.4f;


        _ctx.PlayerRb.transform.localScale = new Vector3(_ctx.PlayerRb.transform.localScale.x, 0.5f, _ctx.PlayerRb.transform.localScale.z);

        // We'll also have to move the player downward
        _ctx.PlayerRb.MovePosition(new Vector3(_ctx.PlayerRb.position.x, _ctx.PlayerRb.position.y - 0.5f, _ctx.PlayerRb.position.z));


        InitializeSubState();
        
    }

    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=blue>Crouch</color> State");
        _ctx.IsCrouched = false;
        _ctx.SpeedMultiplier = 1.0f;
        _ctx.PlayerRb.transform.localScale = new Vector3(_ctx.PlayerRb.transform.localScale.x, startYScale, _ctx.PlayerRb.transform.localScale.z);

        
    }

    public override void FrameUpdate()
    {
        CheckSwitchStates();
    }

    public override void InitializeSubState()
    {
        // Set from where it is first encountered (Sprinting)
        if (_ctx.IsSliding)
        {
            SetSubState(_factory.Sliding());
            return;
        }

        // While grounded and crouched, not moving -> Idle
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
        
    }

}
