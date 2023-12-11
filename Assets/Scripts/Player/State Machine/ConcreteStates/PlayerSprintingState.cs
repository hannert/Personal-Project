using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintingState : PlayerWalkingState
{
    public PlayerSprintingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        // Player is not holding shift
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
            {
                SwitchState(player.playerIdleState);
                return true;
            }
            // Let go of shift but still holding onto a button
            else if (playerStateMachine.horizontalInput != 0 || playerStateMachine.verticalInput != 0)
            {
                SwitchState(player.playerIdleState);
                return true;
            }
           
        }
        return false;
    }

    public override void EnterState()
    {
        playerStateMachine.playerAnim.SetBool("isSprinting", true);

    }

    public override void ExitState()
    {
        playerStateMachine.playerAnim.SetBool("isSprinting", false);

    }

    public override void FrameUpdate()
    {
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void PhysicsUpdate()
    {
        // Move player
        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(playerStateMachine.projectedPos.x, 0, playerStateMachine.projectedPos.z);
        var tempCamera = new Vector3(playerStateMachine.camera.transform.position.x, 0, playerStateMachine.camera.transform.position.z);

        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(playerStateMachine.horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, playerStateMachine.verticalInput);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        // Based on Freya Holmers rotation vector video
        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;

        playerStateMachine.projectedPos += endDirection * playerStateMachine.sprintSpeed * Time.fixedDeltaTime;

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            playerStateMachine.playerRb.MoveRotation(testFinalRotation);
        }

        // Check if player is gonna collide into anything
        //var newPos = checkFuturePosition(endDirection);

        int wallCollisionNum = PlayerUtilities.checkWallCollision(playerStateMachine.wallColliders, playerStateMachine.playerCap, playerStateMachine.projectedPos);

        if (wallCollisionNum > 0)
        {

            playerStateMachine.projectedPos = PlayerUtilities.checkFuturePosition(
                endDirection, playerStateMachine.projectedPos, playerStateMachine.playerRb, playerStateMachine.playerCap, playerStateMachine.speed);


        }



        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        CheckSwitchStates();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
