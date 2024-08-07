using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CombatBase", order = 1)]
public class CombatBaseObject : ScriptableObject
{
    public string attackName;

    [field: SerializeField]
    public CombatBaseObject NextMove { get; private set; }

    /// <summary>
    /// Animation associated with this move
    /// </summary>
    [field: SerializeField]
    public AnimationClip Animation { get; private set; }

    /// <summary>
    /// Time it takes for the next move to be considered
    /// </summary>
    [field: SerializeField]
    public float MinLinkTime { get; private set; }

    /// <summary>
    /// Maximum time for link: Only when minLinkTime has passed, player can advance
    /// </summary>
    [field: SerializeField]
    public float MaxLinkTime { get; private set; }

    /// <summary>
    /// Multiply base damage of weapon to this value eg(1st move - 1.0f, 2nd move - 1.10f for a 10% increase damage in the second move)
    /// </summary>
    [field: SerializeField]
    public float DamageMultiplier { get; private set; }

    // ! Movement ?
    public Vector3 AddForce;

    public ForceMode ForceMode;
}
