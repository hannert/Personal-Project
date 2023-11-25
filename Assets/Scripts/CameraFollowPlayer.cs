using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject Player;
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
        transform.position = new Vector3(xOffset + Player.transform.position.x, yOffset + Player.transform.position.y, zOffset + Player.transform.position.z);
    }
}
