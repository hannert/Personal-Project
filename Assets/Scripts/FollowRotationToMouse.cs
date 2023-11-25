using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotationToMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13));
        var _direction = (worldMousePosition - transform.position).normalized;
        var awesome = Quaternion.LookRotation(_direction);
        //awesome.SetEulerAngles(new Vector3(0, awesome.eulerAngles.y, 0));
        awesome = Quaternion.Euler(0, awesome.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, awesome, Time.deltaTime * 6);
    }
}
