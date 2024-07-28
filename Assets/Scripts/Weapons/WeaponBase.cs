using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public float baseDamage;

    public CombatBaseObject[] moveset;

    public CombatBaseObject[] GetMoveset() {
        return moveset;
    }

    public void Attack() { }

    public void Block() { }

}
