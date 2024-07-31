using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public float baseDamage;

    public CombatBaseObject[] moveset;

    public CombatBaseObject[] GetMoveset() {
        return moveset;
    }
    private int currentMoveIndex;

    private CombatBaseObject currentMove { get; set; }

    public bool NextMove() {
        if ((++currentMoveIndex) >= moveset.Length) {
            return true;
        }
        currentMove = moveset[currentMoveIndex++ % moveset.Length];

        return false;
    }

    public CombatBaseObject GetCurrentMove() {
        return currentMove;
    }

    public void ResetCombo() {
        currentMoveIndex = 0;
        currentMove = moveset[0];
    }


    public void Attack() { }

    public void Block() { }

}
