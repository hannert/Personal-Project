using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotateSpeed = 5.0f;
    public float zoomspeed = 2.0f;
    public bool rightClickActive = false;

    private float minHorizontalAngle = -360;
    private float maxHorizontalAngle = 360;

    private float minVerticalAngle = -90;
    private float maxVerticalAngle = 90;

    public float distToPlayer = 15.0f;
    private float maxZoom = 15.0f;

    public GameObject player;
    private float playerYHalf;
    public float cameraYOffset = 0f;
    public float horizontalInput;
    public float verticalInput;
    public float zoom = 15.0f;

    //Keep track of where the camera would be if it could clip into everything
    private Vector3 ghostPoint;


    public bool belowGround = false;
    public float groundOffset;



    // Start is called before the first frame update
    void Start()
    {
        // Show the mouse 
        Cursor.visible = true;
        playerYHalf = player.GetComponent<CapsuleCollider>().center.y + cameraYOffset;

    }

    // Update is called once per frame
    void Update()
    {
        CheckForCollider();
        if (rightClickActive)
        {
            // Drag left and right , -360 to 360
            horizontalInput += Input.GetAxis("Mouse X") * rotateSpeed;
            horizontalInput = Mathf.Clamp(horizontalInput, minHorizontalAngle, maxHorizontalAngle);
            if (horizontalInput == minHorizontalAngle || horizontalInput == maxHorizontalAngle) horizontalInput = 0;

            // Drag up and down , -90 to 90 but should be bounded by ground or hard objects
            verticalInput += Input.GetAxis("Mouse Y") * rotateSpeed;
            verticalInput = Mathf.Clamp(verticalInput, minVerticalAngle, maxVerticalAngle);

        }
        else
        {
            
        }

        // Get player scrollwheel to increase/decrease 
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        zoom += -scrollWheelInput * zoomspeed;
        zoom = Mathf.Clamp(zoom, 1, maxZoom);


        if (Input.GetMouseButtonDown(1)){
            rightClickActive = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            rightClickActive = false;
        }
    }

    private void LateUpdate()
    {
        // Vertical input should move the X axis, Horizontal move Y
        var targetRotation = Quaternion.Euler(verticalInput, horizontalInput, 0);

        // Focal point with addition of a Vector3(Offset)
        var focusPosition = player.transform.position + new Vector3(0, playerYHalf, 0);

        //transform.position = focusPosition - targetRotation * new Vector3(0, 0, 15);

        var tempPos = focusPosition - targetRotation * new Vector3(0, 0, zoom);

        // The ghost point shares the position, it is only the cameras true position that is modified after
        ghostPoint = tempPos;


        if (belowGround == true)
        {
            //Debug.Log("LateUpdate belowground " + groundOffset);
            tempPos.y = 0.3f;
            transform.position = tempPos;
            transform.LookAt(player.transform);
        } else
        {
            // Scale rotation to same magnitude of distance of focal point?
            transform.position = tempPos;
            transform.rotation = targetRotation;
        }

        
    }

    private void CheckForCollider()
    {
        Ray ray = new Ray(ghostPoint, Vector3.up);
        Debug.DrawRay(ghostPoint, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            belowGround = true;
            groundOffset = hit.distance;
        }
        else
        {
            belowGround = false;
        }
        

    }
}
