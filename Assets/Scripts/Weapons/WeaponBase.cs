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

    private List<Collider> hitEntities = new List<Collider>();


    public bool NextMove() {
        hitEntities.Clear();
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
        hitEntities.Clear();
    }

    /// <summary>
    /// Function to add entity's collider to internal list if hit by weapon movement
    /// </summary>
    /// <param name="hitEntity"> The collider of the hit object </param>
    /// <returns>
    ///     true - if entity was not already hit
    ///     <br/>
    ///     false - if entity was already hit
    /// </returns>
    public bool AddToHit(Collider hitEntity) {
        if (hitEntities.Contains(hitEntity)){
            return false;
        } else {
            hitEntities.Add(hitEntity);
        }
        
        return true;
    }


    public void Attack() { }

    public void Block() { }

}
