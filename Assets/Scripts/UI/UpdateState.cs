using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateState : UIBase
{

    // Update is called once per frame
    void FixedUpdate()
    {
        string buffer = "";
        var tempStateHolder = player.StateMachine.CurrentPlayerState;
        buffer += tempStateHolder?.GetName();
        tempStateHolder = tempStateHolder?.GetSubState();
        while (tempStateHolder != null)
        {
            buffer += " " + tempStateHolder.GetName();
            
            tempStateHolder = tempStateHolder.GetSubState();

        }
        text.text = buffer;
    }
}
