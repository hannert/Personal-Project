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
        Debug.Log(_psm.playerRb.position);
        _psm.projectedPos = snapToGround(_psm.playerRb.position);
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
        if (PlayerUtilities.checkGroundCollision(_psm.groundColliders, _psm.playerCap) == 0)
        {
            _psm.onGround = false;
        }
        CheckSwitchStates();
    }

   

    // Function should snap the player to the ground when its within a certain distance from landing
    private Vector3 snapToGround(Vector3 currentTrajectedPosition)
    {
        // Use whatever is inside psm.groundColliders
        if (_psm.groundColliders[0] != null)
        {
            var groundObject = _psm.groundColliders[0];
            // get the closest point from the feet of our player to the contacted collider
            var contactPoint = groundObject.ClosestPoint(_psm.playerRb.position);
            Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, contactPoint.y, currentTrajectedPosition.z);
            return projectedPos;
        }

        return currentTrajectedPosition;

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
            //_psm.currentFallVelocity = _psm.jumpVelocity;
            _psm.yVelocity = new Vector3(0, _psm.jumpVelocity, 0);
            _psm.distanceFromCameraAtJump = _psm.camera.transform.position - _psm.playerRb.position ;
            SwitchState(player.playerAirState);
        }
        // Roll from grounded state

    }
}
