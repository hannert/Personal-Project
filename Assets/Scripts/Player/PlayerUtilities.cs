﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Helper class that contains functions regarding player movement and status
/// </summary>
public static class PlayerUtilities
{
    static float maxAngle = 70;

    /// <summary>
    /// Check for ground collision with a sphere at the player's feet
    /// </summary>
    /// <param name="groundColliders"></param>
    /// <param name="playerCap"></param>
    /// <returns>The number of valid colliders that are hit</returns>
    public static int CheckGroundCollision(Collider[] groundColliders, CapsuleCollider playerCap)
    {
        Array.Clear(groundColliders, 0, groundColliders.Length);


        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint = playerCap.center + Vector3.down * (playerCap.height / 2 - (playerCap.radius - 0.2f)) * 1.1f; // Feet point

        var point = playerCap.transform.TransformPoint(localPoint);

        DebugExtension.DebugWireSphere(point, playerCap.radius * 0.9f, Time.fixedDeltaTime);
        int numColliders = Physics.OverlapSphereNonAlloc(point, playerCap.radius * 0.9f, groundColliders, LayerMask.GetMask("Ground", "Wall"));
        return numColliders;

    }

    /// <summary>
    /// Checks for wall collision at projectedPosition with an OverlapCapsule method 
    /// </summary>
    /// <param name="wallColliders"></param>
    /// <param name="playerCap"></param>
    /// <param name="projectedPosition"></param>
    /// <returns>The number of valid wall colliders that are overlapping the projected capsule</returns>
    public static int checkWallCollision(Collider[] wallColliders, CapsuleCollider playerCap, Vector3 projectedPosition)
    {
        var localPoint1 = projectedPosition - Vector3.down * (playerCap.height / 2 - playerCap.radius);
        var localPoint2 = projectedPosition + Vector3.down * (playerCap.height / 2 - playerCap.radius);

        // Perhaps we dont need these for our application
        var point1 = playerCap.transform.TransformPoint(localPoint1);
        var point2 = playerCap.transform.TransformPoint(localPoint2);


        int numColliders = Physics.OverlapCapsuleNonAlloc(localPoint1, localPoint2, playerCap.radius, wallColliders, LayerMask.GetMask("Wall"));

        return numColliders;
    }

    /// <summary>
    /// Checks if player will collide when 'surfing' across a wall. Applicable when moving the player on the X & Z plane. 
    /// </summary>
    /// <param name="endDirection"></param>
    /// <param name="projectedPos"></param>
    /// <param name="playerRb"></param>
    /// <param name="playerCap"></param>
    /// <param name="speed"></param>
    /// <returns>A Vector3 of a valid position, current position if no valid position (no change).</returns>
    public static Vector3 CheckFuturePosition(Vector3 endDirection, Vector3 projectedPos, Rigidbody playerRb, CapsuleCollider playerCap, float speed)
    {
        var pointBehindPlayer = -endDirection.normalized + playerRb.position + (Vector3.up * (playerCap.height / 2));
        // Fire ray BEHIND player torwards direction player is FACING to get the ray's hit point to calculate player position AFTER

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
            //           ^
            //            \
            //             \
            //              \
            //             ┌─┐
            //             └─┘


            //───-─────┬─┬── Now we should move it based on speed and angle hit 
            //     <-- └─┘
            //           ^
            //            \
            //             \
            //              \
            //             ┌─┐
            //             └─┘



            // We should also capsule cast its proposed position, no move if capsulecast collides with another wall

            var distanceVector = rotatedNormal * speed * Time.fixedDeltaTime;
            var distance = Vector3.Distance(Vector3.zero, distanceVector);
            var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - playerCap.radius);
            // Have the point SLIGHTLY more UNDER the player collider
            var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - playerCap.radius);// Above point


            var point1 = playerCap.transform.TransformPoint(localPoint1);
            var point2 = playerCap.transform.TransformPoint(localPoint2);

            Vector3 endingPosition = new Vector3(
               playerRb.position.x,
               projectedPos.y,
               playerRb.position.z);


            // If the projected CapsuleCast of the player into a wall returns false, we move. Otherwise the position to move to is unchanged
            if (!Physics.CapsuleCast(point1, point2, playerCap.radius, rotatedNormal, distance + playerCap.radius, LayerMask.GetMask("Wall")))
            {
                endingPosition += distanceVector;
            }


            return endingPosition;
        }



        return projectedPos;
    }

    /// <summary>
    /// Collide and Slide algorithim taken from https://www.youtube.com/watch?v=YR6Q7dUz2uk
    /// </summary>
    /// <param name="playerCap"></param>
    /// <param name="vel"></param>
    /// <param name="pos"></param>
    /// <param name="depth"></param>
    /// <param name="skinWidth"></param>
    /// <param name="maxBounces"></param>
    /// <returns></returns>
    public static Vector3 CollideAndSlide(CapsuleCollider playerCap, Vector3 vel, Vector3 pos, int depth, float skinWidth, int maxBounces, bool gravityPass, Vector3 velInit, bool isGrounded)
    {
        if (depth >= maxBounces)
        {
            return Vector3.zero;
        }

        var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - playerCap.radius);
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - playerCap.radius); // Above point

        // Maybe we cast behind the player to avoid some issues
        var point1 = playerCap.transform.TransformPoint(localPoint1); 
        var point2 = playerCap.transform.TransformPoint(localPoint2);
        float dist = vel.magnitude + skinWidth;
        
        RaycastHit hit;

        Debug.DrawRay(pos + playerCap.center, vel.normalized);
        if (Physics.CapsuleCast(point1, point2, playerCap.radius - skinWidth, vel.normalized, out hit, dist, LayerMask.GetMask("Wall", "Ground"))){
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftover = vel - snapToSurface;

            float angle = Vector3.Angle(Vector3.up, hit.normal);

            // Ensures we have enough room to perform our collision check properly
            if (snapToSurface.magnitude <= skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            // Normal ground / walkable slope
            if(angle <= maxAngle)
            {
                if(gravityPass)
                {
                    return snapToSurface;
                }
                leftover = ProjectAndScale(leftover, hit.normal);
            } 
            // Wall or steep slope
            else
            {
                float scale = 1 - Vector3.Dot(new Vector3(hit.normal.x, 0, hit.normal.z).normalized, -new Vector3(velInit.x, 0, velInit.z).normalized);

                if(isGrounded && !gravityPass)
                {
                  leftover = ProjectAndScale(new Vector3(leftover.x, 0, leftover.z),
                        new Vector3(hit.normal.x, 0, hit.normal.z)).normalized;
                    leftover *= scale;
                }
                else
                {
                    leftover = ProjectAndScale(leftover, hit.normal) * scale;
                }
                
                Debug.DrawRay(pos + playerCap.center + Vector3.up, leftover.normalized, Color.blue);
                
            }


            return snapToSurface + CollideAndSlide(playerCap, leftover, pos + snapToSurface, depth + 1, skinWidth, maxBounces, gravityPass, velInit, isGrounded);

        }
        return vel;
    }



    public static Vector3 ProjectAndScale(Vector3 vector, Vector3 normal)
    {
        float mag = vector.magnitude;
        vector = Vector3.ProjectOnPlane(vector, normal).normalized;
        vector *= mag;
        return vector;
    }

    public static Vector3 moveOnSlope(Rigidbody playerRb, CapsuleCollider playerCap, Vector3 vel, Vector3 pos, float skinWidth)
    {
        // Player will already be 'wedged' into the ground with the ground check collision sphere
        // We should RAYCAST down from the player's midpoint and then project our velocity vector onto the normal of the surface hit
        // Point to cast from = playerCap.center

        // Capsule cast from ABOVE the player
        var distanceOffset = Vector3.up * (playerCap.height / 2);

        var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - (playerCap.radius));
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - (playerCap.radius));
        var point1 = playerCap.transform.TransformPoint(localPoint1) + distanceOffset;
        var point2 = playerCap.transform.TransformPoint(localPoint2) + distanceOffset;


        // Should this be running forever (?) since we can project on the normal on a flat plane as well

        //if (Physics.CapsuleCast(point1, point2, playerCap.radius, Vector3.down, out RaycastHit hit, 1.0f, LayerMask.GetMask("Wall", "Ground"))){
        //    Debug.Log("Yippee!!");

        //    var finalVelocity = Vector3.ProjectOnPlane(vel, hit.normal);
        //    Debug.DrawRay(playerRb.position + (Vector3.up), finalVelocity, Color.blue);
        //    return finalVelocity;
        //}
        if (Physics.Raycast(playerRb.position + playerCap.center , Vector3.down, out RaycastHit hit, playerCap.height + skinWidth, LayerMask.GetMask("Wall", "Ground")))
        {
            var finalVelocity = Vector3.ProjectOnPlane(vel, hit.normal);
            Debug.DrawRay(playerRb.position + (Vector3.up), finalVelocity, Color.blue);
            return finalVelocity;
        }



        return vel;
    }


    public static Vector3 slideOnSlope(CapsuleCollider playerCap, Vector3 vel, Vector3 pos, float skinWidth)
    {
        // We should capsule cast from behind the player =)
        var oppositeDirection = -vel.normalized;
        var backwardsOffset = oppositeDirection * (skinWidth + playerCap.radius);

        var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - (playerCap.radius));// Above point
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - (playerCap.radius)); 
        var point1 = playerCap.transform.TransformPoint(localPoint1) + backwardsOffset;
        var point2 = playerCap.transform.TransformPoint(localPoint2) + backwardsOffset;

        Debug.DrawRay(point1, vel.normalized);

        float dist = vel.magnitude + skinWidth + 0.4f + backwardsOffset.magnitude;
        RaycastHit hit;

        if (Physics.CapsuleCast(point1, point2, playerCap.radius + skinWidth, vel.normalized, out hit, dist, LayerMask.GetMask("Ground")))
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance);

            // Ensures we have enough room to perform our collision check properly
            if (snapToSurface.magnitude <= skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            Vector3 answer  = Vector3.ProjectOnPlane(snapToSurface, hit.normal);
            return answer;

        }
        return Vector3.zero;
    }

    public static Vector3 slideDownSlope(Rigidbody playerRb, CapsuleCollider playerCap, Vector3 vel, Vector3 pos, float skinWidth)
    {
        // We have to cast the player DOWN a certain distance of where the player intends to go 
        float distanceToMove = vel.magnitude;
        var velNormalized = vel.normalized;

        // Should be ahead of player given our player pos and distance 
        var distanceOffset = velNormalized * (skinWidth + playerCap.radius);

        var localPoint1 = (Vector3.up * 0.2f) + playerCap.center - Vector3.down * (playerCap.height / 2 - (playerCap.radius));// Above point
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - (playerCap.radius));
        var point1 = playerCap.transform.TransformPoint(localPoint1) + distanceOffset;
        var point2 = playerCap.transform.TransformPoint(localPoint2) + distanceOffset;

        var groundPoint = getGroundNormal(playerRb, playerCap);
        Vector3 answer = Vector3.ProjectOnPlane(vel.normalized, groundPoint).normalized;
        return answer;

    }

    public static Vector3 getGroundPoint(Rigidbody playerRb, CapsuleCollider playerCap)
    {
        // We shoot a ray from the midpoint of the player to avoid faulty positions
        if (Physics.Raycast(playerRb.transform.position + new Vector3(0, playerCap.height / 2), Vector3.down, out RaycastHit hit, 5.0f, LayerMask.GetMask("Ground")))
        {
            if (hit.distance < 2f)
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }

    public static Vector3 getGroundNormal(Rigidbody playerRb, CapsuleCollider playerCap)
    {
        // We shoot a ray from the midpoint of the player to avoid faulty positions
        if (Physics.Raycast(playerRb.transform.position + new Vector3(0, playerCap.height / 2), Vector3.down, out RaycastHit hit, 5.0f, LayerMask.GetMask("Ground")))
        {
            if (hit.distance < 2f)
            {
                return hit.normal;
            }
        }
        return Vector3.zero;
    }
    /// <summary>
    ///  Takes in an euler angle from the player's rotation (-180, 180) (L, R) Respectively. Transforms it into a direction Vector3 the player is facing in.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns>The directional Vector3 the player is currently facing (Forward).</returns>
    public static Vector3 getDirectionFromOrigin(float angle)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float z = Mathf.Sin(angleInRadians);

        if(angle < 0)
        {
            return new Vector3(x, 0, z).normalized;
        }

        return new Vector3(z, 0, x).normalized;
    }

    /// <summary>
    /// Takes in player position, reference and player input values to calculate direction the player wants to move to based on reference.
    /// </summary>
    /// <param name="projectedPosition"></param>
    /// <param name="referencePosition"></param>
    /// <param name="horizontalInput"></param>
    /// <param name="verticalInput"></param>
    /// <returns>A Vector3 of the direction the player wants to move to.</returns>
    /// 
    public static Vector3 GetDirectionFromCamera(Vector3 projectedPosition, Vector3 referencePosition, float horizontalInput, float verticalInput)
    {
        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(projectedPosition.x, 0, projectedPosition.z);
        var tempCamera = new Vector3(referencePosition.x, 0, referencePosition.z);

        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, verticalInput);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;

        return endDirection;
    }

    // ------------- Stairs Functionality

    // We need a foot height raycast to detect if anything is in the way of our player - lowerRaycast
    // We also need a step height raycast to detect if the object we are hitting with the lowerRay is actually a step and not a wall 
    // If step, go up
    // If wall, do nothing
    

    // variables:
    // Position to raycast from: 
    // Player rigidbody

    public static void stepCast(Rigidbody playerRb, Vector3 lowerRaycast, Vector3 upperRaycast, Vector3 playerForward)
    {
        RaycastHit hitLower;
        Debug.DrawRay(lowerRaycast, playerForward, Color.red);
        if (Physics.Raycast(lowerRaycast, playerForward, out hitLower, 0.6f, LayerMask.GetMask("Ground")))
        {
            Debug.Log("Lower Raycast hit!");
            RaycastHit hitUpper;
            Debug.DrawRay(upperRaycast, playerForward, Color.blue);
            if (Physics.Raycast(upperRaycast, playerForward, out hitUpper, 0.9f))
            {
                Debug.Log("Hit stairs!");
                playerRb.AddForce(Vector3.up *70f, ForceMode.Force);
                //playerRb.position -= new Vector3(0f, -0.5f, 0f);
            }
        }
    }


    public static Vector3 GetInputDirection(PlayerStateMachine _psm)
    {
        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(_psm.PlayerRb.position.x, 0, _psm.PlayerRb.position.z);
        var tempCamera = new Vector3(_psm.Camera.transform.position.x, 0, _psm.Camera.transform.position.z);

        if (_psm.IsLockedOn)
        {
            // the locked on object will become the 'player'
            tempPlayer = new Vector3(_psm.Camera.lockOnFocusObject.transform.position.x, 0, _psm.Camera.lockOnFocusObject.transform.position.z);
            tempCamera = new Vector3(_psm.PlayerRb.position.x, 0, _psm.PlayerRb.position.z);
        }


        // Get the direction from the object (camera) to the reference (player)
        var objectToReferenceDirection = (tempPlayer - tempCamera).normalized;


        // Get horizontal and vertical components from input ------

        // Already normalized ( 0 - 1 value for x and z )
        // Use horizontal input, 'a' or 'd' key
        var horizontalVector = new Vector3(_psm.HorizontalInput, 0, 0);

        // Use vertical input, 'w' or 's' key
        var verticalVector = new Vector3(0, 0, _psm.VerticalInput);

        // Currently overriding the horizontal component with the Axis Raw as our own input smoother still needs work (_psm.iput)
        // TODO: Change back to custom input
        horizontalVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        verticalVector = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));

        // ------


        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(objectToReferenceDirection.z, 0, -objectToReferenceDirection.x);

        // The direction the player's key input is pointing towards
        var endDirection = betweenVector.x * perpVector + betweenVector.z * objectToReferenceDirection;

        return endDirection;
    }



}
