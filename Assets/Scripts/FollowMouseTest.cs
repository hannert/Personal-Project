using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseTest : MonoBehaviour
{
    // Follows the mouse and translates its position to an object
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13));
        //Debug.Log(worldMousePosition.ToString());
        transform.position = worldMousePosition;
    }
}
