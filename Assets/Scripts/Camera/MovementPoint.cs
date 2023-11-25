using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementPoint : MonoBehaviour
{

    private CameraController cameraInfo;

    // Start is called before the first frame update
    void Start()
    {
        cameraInfo = GameObject.Find("Camera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        var targetRotation = Quaternion.Euler(0, cameraInfo.horizontalInput, 0);

        transform.rotation = targetRotation;
    }
}
