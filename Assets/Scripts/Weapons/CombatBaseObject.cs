using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CombatBase", order = 1)]
public class CombatBaseObject : ScriptableObject
{
    public string attackName;

    public CombatBaseObject nextMove;
    public Animation animation;

    // Time available for player to input for the nextMove
    public float linkTime;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
