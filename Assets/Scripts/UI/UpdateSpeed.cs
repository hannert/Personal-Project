using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateSpeed : UIBase
{
    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = "Speed:" + player.currentSpeed;
    }
}
