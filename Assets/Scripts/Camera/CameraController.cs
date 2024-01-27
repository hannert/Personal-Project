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

    // Set to something less than the 90 since it's considered a straight up POV, which messeses up the movement
    private float minVerticalAngle = -89.75f;
    private float maxVerticalAngle = 89.75f;

    public float distToPlayer = 15.0f;
    private float maxZoom = 15.0f;

    [Tooltip("The distance between the ground and the camera if there is a hard object btwn")]
    public float groundGapSize = 0.3f;


    public float lockOnXOffset = 0f;
    public float lockOnYOffset = 0f;
    public float lockOnZOffset = 0f;
    public bool isLockedOn;
    public GameObject lockOnReference;
    public GameObject lockOnFocusObject;
    public GameObject player;
    private Rigidbody playerRb;
    private float playerYHalf;
    public float cameraYOffset = 0f;
    public float horizontalInput;
    public float verticalInput;
    public float zoom = 15.0f;

    //Keep track of where the camera would be if it could clip into everything
    private Vector3 ghostPoint;

    [Tooltip("Is any object between the player and the camera?")]
    private bool clearPath = true;

    [Tooltip("Stored point of contact of the first object hit btwn player and camera")]
    private Vector3 rayPathHitPoint = Vector3.zero;

    [Tooltip("Stored normal of contact of the first object hit between player and camera")]
    private Vector3 rayPathHitNormal = Vector3.zero;

    public float groundOffset;



    // Start is called before the first frame update
    void Start()
    {
        // Show the mouse 
        Cursor.visible = true;
        playerYHalf = player.GetComponent<CapsuleCollider>().center.y + cameraYOffset;
        playerRb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // First we check if anything is touching the camera 
        CheckForCollider();
        if (rightClickActive && !isLockedOn)
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

    private void FixedUpdate()
    {
        // Vertical input should move the X axis, Horizontal move Y
        var targetRotation = Quaternion.Euler(verticalInput, horizontalInput, 0);

        // Focal point with addition of a Vector3(Offset)
        var focusPosition = player.transform.position + new Vector3(0, playerYHalf, 0);
        var tempPos = focusPosition - targetRotation * new Vector3(0, 0, zoom);

        if (isLockedOn)
        {
            targetRotation = Quaternion.LookRotation(lockOnFocusObject.transform.position - transform.position, Vector3.up);
            focusPosition += lockOnFocusObject.transform.position;
            var awayFromPlayer = (lockOnFocusObject.transform.position - playerRb.transform.position).normalized;
            var cameraDirection = new Vector3(awayFromPlayer.z, awayFromPlayer.y, -awayFromPlayer.x);
            tempPos = playerRb.transform.position + cameraDirection + new Vector3(0, playerYHalf + 1.5f, 0) - (5 * awayFromPlayer);
        }

        // The ghost point shares the position, it is only the cameras true position that is modified after
        ghostPoint = tempPos;

        // If not clear path, something is betwixt the player and the camera
        if (!clearPath)
        {
            transform.position = rayPathHitPoint + (rayPathHitNormal * groundGapSize);
            transform.LookAt(player.transform);
        }
        else
        {
            Vector3 vel = Vector3.zero;
            // Scale rotation to same magnitude of distance of focal point?
            if (!isLockedOn)
            {
                transform.position = tempPos;
            }
            if (isLockedOn)
            {
                transform.position = Vector3.SmoothDamp(transform.position, tempPos, ref vel, 0.05f);
            }
            transform.rotation = targetRotation;


        }
        
    }
    
    /// <summary>
    /// Function to check if the camera is below the ground
    /// </summary>
    private void CheckForCollider()
    {
        // Get a ray from the midpoint of the player to the ghostPoint to get hard objects btwn the player & camera
        Vector3 playerMidPoint = playerRb.transform.position + new Vector3(0, playerYHalf);
        Ray playerToCamera = new Ray(playerMidPoint, (transform.position - playerMidPoint).normalized);
        Debug.DrawRay(playerMidPoint, (transform.position - playerMidPoint).normalized);

        if (Physics.Linecast(playerMidPoint, ghostPoint, out RaycastHit collisionHit))
        {
            Debug.DrawRay(collisionHit.point, Vector3.up);

            clearPath = false;
            rayPathHitPoint = collisionHit.point;
            rayPathHitNormal = collisionHit.normal;
        } else
        {
            clearPath = true;
        }
    }

    public bool toggleLockOn()
    {
        isLockedOn = !isLockedOn;
        return isLockedOn;
    }
    public void ChangeLockOn()
    {

    }
}
