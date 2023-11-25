using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject toFollow;
    public float xOffset = 0f;
    public float yOffset = 0f;
    public float zOffset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13));
        var _direction = (worldMousePosition - transform.position).normalized;
        //var quatToMouse = Quaternion.LookRotation(_direction);
        //var yAngle = quatToMouse.eulerAngles.y;

        var offsetPt = GetPoint(0);




        transform.position = new Vector3(
            xOffset + toFollow.transform.position.x + offsetPt.x,
            yOffset + toFollow.transform.position.y,
            zOffset + toFollow.transform.position.z + offsetPt.z
            );
    }

    Vector3 GetPoint(float angle)
    {
        float playerAngle = toFollow.transform.rotation.eulerAngles.y;
        //Debug.Log("Player Angle");
        //Debug.Log(playerAngle);
        float ansX;
        float ansZ;

        ansX = 2 * Mathf.Sin(playerAngle * Mathf.Deg2Rad);
        ansZ = 2 * Mathf.Cos(playerAngle * Mathf.Deg2Rad);
        
        //Debug.Log("X and Z");

        //Debug.Log(ansX);
        //Debug.Log(ansZ);
        return new Vector3(ansX, 0.83f, ansZ);
    }
}
