using System;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered grounded state");
        _psm.playerAnim.SetBool("isGrounded", true);
        // Should snap the player to the ground here upon ENTERING
        // Send info to projectedPos
        _psm.onGround = true;
        InitializeSubState();
        Vector3 newPos = snapToGround(_psm.playerRb.position);
        _psm.projectedPos = newPos;
        //Debug.Log("--" + _psm.projectedPos);
        
    }

    public override void ExitState()
    {
        Debug.Log("Exited grounded root state");
        _psm.onGround = false;
        _psm.playerAnim.SetBool("isGrounded", false);

    }

    public override void FrameUpdate()
    {
        getKeyPress();
    }

    public override void InitializeSubState()
    {
        // While grounded, not moving -> Idle
        if(_psm.horizontalInput == 0 && _psm.verticalInput == 0)
        {
            SetSubState(player.playerIdleState);
        } else
        {
            SetSubState(player.playerWalkingState);
        }
        

    }

    public override void PhysicsUpdate()
    {
        // When grounded, check if there is floor beneath to trigger falling state
        if(PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) == 0)
        {
            _psm.onGround = false;
        }
        CheckSwitchStates();
    }

   

    // Function should snap the player to the ground when its within a certain distance from landing
    // But what if we land on a SLANTED surface? -> Currently we keep teleporting up and falling down
    private Vector3 snapToGround(Vector3 currentTrajectedPosition)
    {

        float newYPos = getGroundPoint().y;
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos + 0.01f, currentTrajectedPosition.z);
        return projectedPos;

    }

    // Function should raycast from the player feet downwards to get the worldspace
    // coordinate of the surface they are standing on 
    private Vector3 getGroundPoint()
    {
        // We shoot a ray from the midpoint of the player to avoid faulty positions
        if (Physics.Raycast(_psm.playerRb.transform.position + new Vector3(0, _psm.playerCap.height / 2), Vector3.down, out RaycastHit hit, 5.0f, LayerMask.GetMask("Ground", "Wall")))
        {
            if (hit.distance < 2f)
            {
                return hit.point;
            }            
        }
        return Vector3.zero;
    }

    public override bool CheckSwitchStates()
    {
        if (_psm.onGround == false)
        {
            SwitchState(player.playerAirState);
            return true;
        }

        return false;

    }

    public void getKeyPress()
    {
        // Jump from grounded state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _psm.currentFallVelocity = _psm.jumpVelocity;
            _psm.distanceFromCameraAtJump = _psm.camera.transform.position - _psm.playerRb.position ;
            SwitchState(player.playerAirState);
        }
        // Roll from grounded state

    }
}
