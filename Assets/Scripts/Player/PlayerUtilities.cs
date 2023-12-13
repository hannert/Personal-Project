using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Helper class that contains functions regarding player movement and status
/// </summary>
public class PlayerUtilities
{

    /// <summary>
    /// Check for ground collision with a sphere at the player's feet
    /// </summary>
    /// <param name="groundColliders"></param>
    /// <param name="playerCap"></param>
    /// <returns>The number of valid colliders that are hit</returns>
    public static int checkGroundCollision(Collider[] groundColliders, CapsuleCollider playerCap)
    {
        Array.Clear(groundColliders, 0, groundColliders.Length);

        //We dont need point1 right now, but will be used if we need to use a capsule cast to reflect the players capsule collider size
        var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - (playerCap.radius - 0.2f));
        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - (playerCap.radius - 0.2f)) * 1.1f; // Above point

        var point1 = playerCap.transform.TransformPoint(localPoint1);
        var point2 = playerCap.transform.TransformPoint(localPoint2);

        DebugExtension.DebugWireSphere(point2, playerCap.radius - 0.2f, Time.fixedDeltaTime);
        int numColliders = Physics.OverlapSphereNonAlloc(point2, playerCap.radius, groundColliders, LayerMask.GetMask("Ground"));

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
    public static Vector3 checkFuturePosition(Vector3 endDirection, Vector3 projectedPos, Rigidbody playerRb, CapsuleCollider playerCap, float speed)
    {
        var pointBehindPlayer = -endDirection.normalized + playerRb.position + (Vector3.up * (playerCap.height / 2));
        // Fire ray BEHIND player torwards direction player is FACING to get the ray's hit point to calculate player position AFTER

        if (Physics.Raycast(pointBehindPlayer, endDirection, out RaycastHit hit, 2f, LayerMask.GetMask("Wall")))
        {
            Debug.Log("Hit!");
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

    //
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

    public static void checkRigidBodyOverlap()
    {

    }

    public static void pushRigidBodyOut()
    {

    }


}
