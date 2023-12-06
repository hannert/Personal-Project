using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {

    }

    public override bool CheckSwitchStates()
    {
        if (playerStateMachine.horizontalInput == 0 && playerStateMachine.verticalInput == 0)
        {
            Debug.Log("Entering Idle substate");
            playerStateMachine.playerAnim.SetBool("isWalking", false);
            SwitchState(player.playerIdleState);
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        // Play animation
        Debug.Log("Entered Walking state");
    }

    public override void ExitState()
    {
    }

    public override void FrameUpdate()
    {
        // Take in input

    }

    public override void InitializeSubState()
    {
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

        playerStateMachine.projectedPos += endDirection * playerStateMachine.speed * Time.fixedDeltaTime;

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
            // Fire ray BEHIND player torwards direction player is FACING to get the ray's hit point to calculate player position AFTER

            var pointBehindPlayer = -endDirection.normalized + playerStateMachine.playerRb.position + (Vector3.up * (playerStateMachine.playerCap.height / 2));
            Debug.DrawLine(pointBehindPlayer, playerStateMachine.projectedPos + (Vector3.up * (playerStateMachine.playerCap.height / 2)), Color.red);

            if (Physics.Raycast(pointBehindPlayer, endDirection, out RaycastHit hit, 2f, LayerMask.GetMask("Wall")))
            {

                Vector3 oppoNorm = -endDirection.normalized;
                Vector3 normalAngle = hit.normal;


                // Get info on if angle is left or right or the normal
                var angleBtwn = Vector3.SignedAngle(hit.normal, oppoNorm, Vector3.up);
                var rotatedNormal = Vector3.back;

                // If negative angle, then player is coming in from the RIGHT to the raycast hit
                if (angleBtwn < 0)
                {
                    // Rotate the normal angle CW 
                    rotatedNormal = new Vector3(hit.normal.z, 0, -hit.normal.x);
                    Debug.DrawLine(hit.point, hit.point + rotatedNormal, Color.blue, 1f);

                }

                // If positive angle, then player is coming in from the LEFT to the raycast hit
                if (angleBtwn > 0)
                {
                    // Rotate the normal angle CCW
                    rotatedNormal = new Vector3(-hit.normal.z, 0, hit.normal.x);
                    Debug.DrawLine(hit.point, hit.point + rotatedNormal, Color.red, 1f);

                }
                // projected position is now in FRONT of the raycast hit with player radius accounted for 
                //───-─────┬─┬── <- Player at wall
                //         └─┘
                //           x
                //            x
                //             x
                //              x
                //             ┌─┐
                //             └─┘

                //playerStateMachine.projectedPos = new Vector3(
                //    hit.point.x + normalAngle.x * (playerStateMachine.playerCap.radius),
                //    playerStateMachine.projectedPos.y,
                //    hit.point.z + normalAngle.z * (playerStateMachine.playerCap.radius));


                //───-─────┬─┬── Now we should move it based on speed and angle hit 
                //     <-- └─┘
                //           x
                //            x
                //             x
                //              x
                //             ┌─┐
                //             └─┘


                playerStateMachine.projectedPos = new Vector3(
                    playerStateMachine.playerRb.position.x,
                    playerStateMachine.projectedPos.y,
                    playerStateMachine.playerRb.position.z
                    ) + rotatedNormal * playerStateMachine.speed * Time.fixedDeltaTime;

                //playerStateMachine.projectedPos = new Vector3(
                //    hit.point.x + normalAngle.x * (playerStateMachine.playerCap.radius),
                //    playerStateMachine.projectedPos.y,
                //    hit.point.z + normalAngle.z * (playerStateMachine.playerCap.radius));
                //Debug.Log(playerStateMachine.projectedPos);
            }


        }




        //Debug.Log(playerStateMachine.projectedPos);

        playerStateMachine.playerRb.MovePosition(playerStateMachine.projectedPos);
        CheckSwitchStates();
    }

    private Vector3 checkFuturePosition(Vector3 direction)
    {
        var localPoint1 = playerStateMachine.playerCap.center - Vector3.down * (playerStateMachine.playerCap.height / 2 - playerStateMachine.playerCap.radius);
        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint2 = playerStateMachine.playerCap.center + Vector3.down * (playerStateMachine.playerCap.height / 2 - playerStateMachine.playerCap.radius);// Above point


        var point1 = playerStateMachine.playerCap.transform.TransformPoint(localPoint1);
        var point2 = playerStateMachine.playerCap.transform.TransformPoint(localPoint2);

        if (Physics.CapsuleCast(point1, point2, playerStateMachine.playerCap.radius, direction, out RaycastHit hit, 5f, LayerMask.GetMask("Ground"))){
            Debug.DrawLine(playerStateMachine.playerCap.transform.position + new Vector3(0, playerStateMachine.playerCap.height/2), hit.point, Color.red);
            var distanceToProjected = Vector3.Distance(playerStateMachine.projectedPos, playerStateMachine.playerRb.position);
            Debug.Log(distanceToProjected);
            Debug.Log(hit.distance);
            if (hit.distance < 1)
            {

                Vector3 oppoNorm = -direction.normalized;
                Debug.Log(playerStateMachine.projectedPos);
                return new Vector3(
                    hit.point.x + oppoNorm.x * (playerStateMachine.playerCap.radius),
                    playerStateMachine.projectedPos.y, 
                    hit.point.z + oppoNorm.z * (playerStateMachine.playerCap.radius));

                


            }

        }
        return Vector3.zero;


    }




}
