using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//var DEBUG = 1;

public class PlayerControllerHybrid : MonoBehaviour
{
    // Maybe we swap to the standard wasd and make a parry focused game
    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float rollDistance = 4f;
    public Rigidbody playerRb;
    public CapsuleCollider playerCap;
    private CameraController camera;

    [SerializeField]
    private float horizontalInput;
    [SerializeField]
    private float verticalInput;

    // TODO : Seperate these into a state machine for modularity 
    public bool onGround = false;
    public bool canJump = true;
    public bool isJumping = false;
    public bool isFalling = false;
    public bool isSprinting = false;
    public bool snapFlag = false;
    
    public float maxFallSpeed = 30.0f;
    public float currentFallVelocity = 0.0f;
    public float jumpVelocity = 20.0f;
    public float gravity = -5.0f;
    private float currentYPos = 0f;
    private RaycastHit[] hits = new RaycastHit[10];
    private Collider[] groundColliders = new Collider[5];


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCap = GetComponent<CapsuleCollider>();

        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();
        currentYPos = transform.position.y;
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump == true)
            {
                Debug.Log("Jumped");
                playerRb.AddForce(Vector3.up * 30, ForceMode.VelocityChange);
            }
        }

        
        
        currentYPos = transform.position.y;
    }

    // Physics updates go here
    private void FixedUpdate()
    {

        // Get position of the player and the camera without the Y component
        var tempPlayer = new Vector3(transform.position.x, 0, transform.position.z);
        var tempCamera = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);

        // Get the direction from the camera to the player 
        var testDirection = (tempPlayer - tempCamera).normalized;

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, verticalInput);

        // Get the combined vector from horizontal and vertical input
        var betweenVector = (horizontalVector + verticalVector).normalized;

        // Rotate the vector perpendicular
        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        var endDirection = betweenVector.x * perpVector + betweenVector.z * testDirection;

        var endingPosition = playerRb.position + endDirection * speed * Time.fixedDeltaTime;

        // Get rotation of the player to reflect the keys inputted 
        if (endDirection != Vector3.zero)
        {
            var testFinalRotation = Quaternion.LookRotation(endDirection, Vector3.up);
            var finalRotation = testFinalRotation.eulerAngles.normalized;
            //playerRb.rotation = testFinalRotation;
        }
        Debug.Log(endDirection);
        playerRb.AddForce(Physics.gravity * 3, ForceMode.Acceleration);
        playerRb.AddForce(endDirection.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);




    }


}
