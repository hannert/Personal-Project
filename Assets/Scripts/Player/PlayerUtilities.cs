using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Helper class that contains functions regarding player movement and status
/// </summary>
public class PlayerUtilities
{

    // Check the sphere at players feet for ground collision
    // Returns the number of valid colliders that are hit
    public static int checkGroundCollision(Collider[] groundColliders, CapsuleCollider playerCap)
    {
        Array.Clear(groundColliders, 0, groundColliders.Length);

        //We dont need point1 right now, but will be used if we need to use a capsule cast to reflect the players capsule collider size
        var localPoint1 = playerCap.center - Vector3.down * (playerCap.height / 2 - playerCap.radius);
        // Have the point SLIGHTLY more UNDER the player collider
        var localPoint2 = playerCap.center + Vector3.down * (playerCap.height / 2 - playerCap.radius) * 1.1f; // Above point

        var point1 = playerCap.transform.TransformPoint(localPoint1);
        var point2 = playerCap.transform.TransformPoint(localPoint2);

        DebugExtension.DebugWireSphere(point2, playerCap.radius, Time.fixedDeltaTime);
        int numColliders = Physics.OverlapSphereNonAlloc(point2, playerCap.radius, groundColliders, LayerMask.GetMask("Ground"));

        return numColliders;

    }

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

}
