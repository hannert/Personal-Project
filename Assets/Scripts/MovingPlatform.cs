using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool goBackwards;
    public float speed;
    Rigidbody objectRb;
    BoxCollider objectCollider;

    void Start()
    {
        objectRb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
        if (goBackwards)
        {
            objectRb.AddForce(Vector3.back * Time.fixedDeltaTime * speed, ForceMode.VelocityChange);
        }
        else
        {
            objectRb.AddForce(Vector3.forward * Time.fixedDeltaTime * speed, ForceMode.VelocityChange);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.GetMask("Wall"))
        {
            goBackwards = !goBackwards;
        }
    }
}
