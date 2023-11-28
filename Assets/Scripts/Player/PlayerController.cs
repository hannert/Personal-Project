using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//var DEBUG = 1;

public class PlayerController : MonoBehaviour
{
    // Maybe we swap to the standard wasd and make a parry focused game
    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float rollDistance = 4f;
    public Rigidbody playerRb;
    private CameraController camera;


    // TODO : Seperate these into a state machine for modularity 
    public bool onGround = false;
    public bool canJump = true;
    public bool isJumping = false;
    public bool isFalling = false;
    public bool isSprinting = false;

    public float maxFallSpeed = 30.0f;
    public float currentFallVelocity = 0.0f;
    public float jumpVelocity = 20.0f;
    public float gravity = -5.0f;
    private float currentYPos = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();
        currentYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump == true)
            {
                currentFallVelocity = jumpVelocity;
                isJumping = true;
                // JUMP
            }
        }

        
        
        currentYPos = transform.position.y;
    }

    // Physics updates go here
    private void FixedUpdate()
    {
        checkGroundCollision();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(transform.position.x, 0, transform.position.z);
        var tempCamera = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);

        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, verticalInput);
        var testYOffset = new Vector3(0, 2, 0);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        // Based on Freya Holmers rotation vector video
        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;

#if DEBUG
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + testDirection + testYOffset, Color.yellow);
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + perpVector + testYOffset, Color.magenta);
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + endDirection + testYOffset, Color.green);
#endif


        var endingPosition = playerRb.position + endDirection * speed * Time.deltaTime;

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            playerRb.MoveRotation(testFinalRotation);
            //playerRb.MovePosition(playerRb.position + endDirection * speed * Time.deltaTime);
            
        }

        if (isJumping || !onGround)
        {
            endingPosition = applyGravityToVector(endingPosition);
            if (currentFallVelocity < 0) { isFalling = true; }
        }


        playerRb.MovePosition(endingPosition);

        if (horizontalInput != 0 || verticalInput != 0)
        {
            
        }


    }

    // Should return a vector of the position the player should be AFTER gravity is applied in said time unit
    private Vector3 applyGravityToVector(Vector3 currentTrajectedPosition)
    {
        float newYPos = currentYPos + (currentFallVelocity * Time.deltaTime + ((0.5f) * gravity * Time.deltaTime * Time.deltaTime));
        Vector3 projectedPos = new Vector3(currentTrajectedPosition.x, newYPos, currentTrajectedPosition.z);
        currentFallVelocity += gravity * Time.deltaTime;
        currentFallVelocity = Mathf.Clamp(currentFallVelocity, -maxFallSpeed, jumpVelocity);
        return projectedPos;
    }

    // Perhaps we change this to capsule cast
    private void checkGroundCollision()
    {
        Ray ray = new Ray(playerRb.transform.position, Vector3.down * 0.0001f);
        Debug.DrawRay(playerRb.transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(playerRb.transform.position, Vector3.down, out RaycastHit hit))
        {
            if (hit.distance < 0.1f)
            {
                onGround = true;
                isJumping = false;
                canJump = true;
                isFalling = false;
                currentFallVelocity = 0;
            }
            
        }
        else
        {
            // Player is in the air or in the falling state
            onGround = false;
            isFalling = true;

        }
    }



}
