using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float tiltSpeed;
    public float xRotationOffset = 0.0f;
    public float yRotationOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {

        
        // The arrow should mimic the rotation of the player
        Quaternion target = Quaternion.Euler(xRotationOffset, player.transform.rotation.eulerAngles.y + yRotationOffset, 0);
        
        // Slerp for smooth transition into the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * tiltSpeed);

        transform.position = GetPoint(0);
    }

    Vector3 GetPoint(float angle)
    {
        float playerAngle = player.transform.rotation.eulerAngles.y;
        //Debug.Log("Player Angle");
        //Debug.Log(playerAngle);
        float ansX;
        float ansZ;

        ansZ = 1 * Mathf.Cos(playerAngle * Mathf.Deg2Rad);
        ansX = 1 * Mathf.Sin(playerAngle * Mathf.Deg2Rad);
        //Debug.Log("X and Z");

        //Debug.Log(ansX);
        //Debug.Log(ansZ);
        return new Vector3(ansX, 0.83f, ansZ);
    }
}
