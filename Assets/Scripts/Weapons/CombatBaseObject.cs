using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CombatBase", order = 1)]
public class CombatBaseObject : ScriptableObject
{
    public string attackName;

    public CombatBaseObject nextMove;
    public AnimationClip animation;

    // Time available for player to input for the nextMove
    public float linkTime;

    // Multiply base damage of weapon to this value eg(1st move - 1.0f, 2nd move - 1.10f for a 10% increase damage in the second move)
    public float damageMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
