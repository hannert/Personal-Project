
using Unity.Collections;
using UnityEngine;

// Wrapper class for keybinds for more uniform addressing of inputs through the code
public class Keybinds
{

    [ReadOnly] public static KeyCode fire = KeyCode.Mouse0;
    [ReadOnly] public static KeyCode foward = KeyCode.W;
    [ReadOnly] public static KeyCode backward = KeyCode.S;
    [ReadOnly] public static KeyCode left = KeyCode.A;
    [ReadOnly] public static KeyCode right = KeyCode.D;
    [ReadOnly] public static KeyCode jump = KeyCode.Space;
    [ReadOnly] public static KeyCode crouch = KeyCode.LeftControl;
    [ReadOnly] public static KeyCode crouchAlt = KeyCode.RightAlt;
    [ReadOnly] public static KeyCode roll = KeyCode.LeftAlt;
    [ReadOnly] public static KeyCode sprint = KeyCode.LeftShift;
    [ReadOnly] public static KeyCode sprintAlt = KeyCode.RightShift;
    [ReadOnly] public static KeyCode slide = KeyCode.LeftControl;
    [ReadOnly] public static KeyCode slideAlt = KeyCode.Mouse3;
}
