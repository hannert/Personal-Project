using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Maybe we swap to the standard wasd and make a parry focused game
    public float speed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float rollDistance = 4f;
    public Rigidbody playerRb;
    private CameraController camera;

    // When ready to use Coroutines change rollCooldown to float and start the countdown
    // public float timeSinceRoll = 0f;
    public float rollCooldown = 1f; 

    public bool rollReady = true;
    public bool isRolling = false;
    public float timeSinceRoll = 0;
    public float timeToRoll = 1;
    public float timeSpentRolling = 0; //Variable to keep track of Vector3 Lerp
    private Vector3 _startRollPos;
    private Vector3 _endRollPos;

    private float horizontalInput;
    private float verticalInput;

    private readonly float unitCircle = Mathf.Sqrt(2) / 2;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        // Get the cameracontroller component to get a reference to the focal point for rotating the character
        camera = GameObject.Find("Camera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(_startRollPos.ToString());
            Debug.Log(_endRollPos.ToString());

        }
        if (rollReady == false)
        {
            _rollCountdown();
        }

        else
        {

        
            //horizontalInput = Input.GetAxis("Horizontal");
            //verticalInput = Input.GetAxis("Vertical");
            
            //playerRb.AddRelativeForce(Vector3.forward * speed * verticalInput, ForceMode.Force);
            //playerRb.AddRelativeForce(Vector3.right * speed * horizontalInput, ForceMode.Force);


            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    if (rollReady == true && !isRolling)
            //    {

            //        rollReady = false;
            //        isRolling = true;


            //        playerRb.AddForce(Vector3.forward * rollDistance * speed, ForceMode.Impulse);

            //    }
            //}
        }

    }

    // Physics updates go here
    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        var tempPlayer = new Vector3(transform.position.x, 0, transform.position.z);
        var tempCamera = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);

        var testDirection = (tempPlayer - tempCamera).normalized;

        //var testFinalRotation = Quaternion.LookRotation(testDirection, Vector3.up);


        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition

        // Already normalized ( 0 - 1 value for x and z )
        var horizontalVector = new Vector3(horizontalInput, 0, 0);
        var verticalVector = new Vector3(0, 0, verticalInput);
        var testYOffset = new Vector3(0, 2, 0);


        var betweenVector = (horizontalVector + verticalVector).normalized;


        var perpVector = new Vector3(testDirection.z, 0, -testDirection.x);

        // Based on Freya Holmers rotation vector video
        var endVector = betweenVector.x * perpVector + betweenVector.z * testDirection;
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + testDirection + testYOffset, Color.yellow);
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + perpVector + testYOffset, Color.magenta);
        Debug.DrawLine(playerRb.transform.position + testYOffset, playerRb.transform.position + endVector + testYOffset, Color.green);




        var testFinalRotation = Quaternion.LookRotation(endVector, Vector3.up);



        if (horizontalInput != 0 || verticalInput != 0)
        {
            playerRb.MoveRotation(testFinalRotation);
            playerRb.MovePosition(playerRb.position + endVector * speed * Time.deltaTime);
        }


    }


    void _rollCountdown()
    {
        timeSinceRoll += Time.deltaTime;
        if (timeSinceRoll > rollCooldown)
        {
            rollReady = true;
            timeSinceRoll = 0;
        }
    }

    void _moveCountdown()
    {
        timeSpentRolling += Time.deltaTime;

    }

    void roll(Vector3 startPos, Vector3 direction)
    {
        if(timeSpentRolling > timeToRoll)
        {
            transform.position = _endRollPos;
            isRolling = false;
            timeSpentRolling = 0;

        } else
        {
            Debug.Log(timeSpentRolling / timeToRoll);
            transform.position = Vector3.Lerp(startPos, direction, timeSpentRolling / timeToRoll);
            timeSpentRolling += Time.deltaTime;
        }
        
    }



}
