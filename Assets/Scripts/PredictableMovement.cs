using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictableMovement : MonoBehaviour
{
    Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.z > 40)
        {
            transform.position = startingPos;
        }
        else
        {
            transform.Translate(Vector3.forward * Time.fixedDeltaTime);
        }
        
    }
}
