using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponBase : MonoBehaviour
{
    public float baseDamage;

    public CombatMovesetObject movesetObject;

    private CombatMoveset moveset;

    /// <summary>
    /// Index of which combo of the moveset we are on
    /// </summary>
    private int currentComboIndex;

    /// <summary>
    /// Index of which move of the combo we are on
    /// </summary>
    private int currentMoveIndex;

    private CombatCombo currentCombo { get; set; }
    private CombatMove currentMove { get; set;}
    private CombatBaseObject currentAction { get; set; }

    private VisualEffectAsset currentActionSlashVFX { get; set; }

    private float currentActionSlashDelay { get; set; }

    /// <summary>
    /// List of colliders hit with the weapon, used to deal damage once per move
    /// </summary>
    private List<Collider> hitEntities = new List<Collider>();

    public GameObject slashPosition;
    public GameObject slashObject;

    private VisualEffect slashEffect;

    void Awake() {
        moveset = movesetObject.moveset;
        slashEffect = slashObject.GetComponentInChildren<VisualEffect>();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="link"></param>
    /// <returns></returns>
    public void LinkToNewCombo(CombatLink link) {
        Debug.Log("Linking new combo" + link.comboIndex.ToString() + " " + link.moveIndex.ToString());
        // Clear hit colliders array since we are going to a new move
        hitEntities.Clear();

        currentComboIndex = link.comboIndex;
        currentMoveIndex = link.moveIndex;

        currentCombo = moveset.moves[currentComboIndex];
        currentMove = currentCombo.combo[currentMoveIndex];
        currentAction = currentCombo.combo[currentMoveIndex].action;

        currentActionSlashVFX = currentMove.slashAsset;
        currentActionSlashDelay = currentMove.slashDelay;
    }

    /// <summary>
    /// Advances the current move in the current combo
    /// </summary>
    /// <returns></returns>
    public bool NextMove() {
        // If we reach the end of the combo string, return true
        if ((++currentMoveIndex) >= currentCombo.combo.Length) {
            return true;
        }

        // Clear hit colliders array since we are going to a new move
        hitEntities.Clear();
        
        // Advance to the next move in the combo
        currentMove = currentCombo.combo[currentMoveIndex];
        currentAction = currentCombo.combo[currentMoveIndex].action;
        
        return false;
    }


    public float GetSlashDelay() {
        return currentActionSlashDelay;
    }
    public void PlaySlashEffect() {
        slashObject.transform.position = slashPosition.transform.position;
        slashEffect.Play();
    }

    public CombatBaseObject GetCurrentAction() {
        return currentAction;
    }
    public CombatBindsEnum GetCurrentComboKey() {
        return currentCombo.type;
    }


    public CombatLink[] GetCombatLinks() {
        return currentCombo.combo[currentMoveIndex].linkableActions;
    }

    // public void ResetCombo() {
    //     currentMoveIndex = 0;
    //     currentMove = moveset[0];
    //     hitEntities.Clear();
    // }

    public bool Prepare(KeyCode key) {
        // Clear hit colliders array since we are going to a new move
        hitEntities.Clear();

        currentCombo = moveset.GetCombo(key);
        currentMoveIndex = 0;
        currentMove = currentCombo.combo[currentMoveIndex];
        currentAction = currentCombo.combo[currentMoveIndex].action;
        
        currentActionSlashVFX = currentMove.slashAsset;
        currentActionSlashDelay = currentMove.slashDelay;

        return false;
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




}
