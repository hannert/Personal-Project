using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCanJump : UIBase
{

    // Update is called once per frame
    void FixedUpdate()
    {
        string buffer = "";
        var stateMachine = player.stateMachine;
        buffer += "can jump? " + stateMachine.CanJump;
        text.text = buffer;
    }
}
