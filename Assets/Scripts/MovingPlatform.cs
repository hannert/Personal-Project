using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    Rigidbody objectRb;

    void Start()
    {
        objectRb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        objectRb.AddForce(Vector3.forward * Time.fixedDeltaTime * speed, ForceMode.Force);
    }
}
