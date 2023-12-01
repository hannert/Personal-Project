using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



}
