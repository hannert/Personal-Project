using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{

    private Animator swordAnimator;


    // Start is called before the first frame update
    void Start()
    {
        swordAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            swordAnimator.SetTrigger("Swing") ;
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            swordAnimator.SetTrigger("Stab");
        }
    }
}
