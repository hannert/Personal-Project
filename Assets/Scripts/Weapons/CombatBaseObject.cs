using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CombatBase", order = 1)]
public class CombatBaseObject : ScriptableObject
{
    public string attackName;

    public CombatBaseObject nextMove;

    /// <summary>
    /// Animation associated with this move
    /// </summary>
    public AnimationClip animation;

    /// <summary>
    /// Time it takes for the next move to be considered
    /// </summary>
    public float minLinkTime;

    /// <summary>
    /// Maximum time for link: Only when minLinkTime has passed, player can advance
    /// </summary>
    public float maxLinkTime;

    /// <summary>
    /// Multiply base damage of weapon to this value eg(1st move - 1.0f, 2nd move - 1.10f for a 10% increase damage in the second move)
    /// </summary>
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
