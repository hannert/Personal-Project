using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// The logic for weapons in the game
/// </summary>
public class WeaponBase : MonoBehaviour
{
    /// <summary>
    /// Base damage the weapon deals before any modifiers
    /// </summary>
    public float baseDamage;

    /// <summary>
    /// The scriptable object holding the moveset for this weapon
    /// </summary>
    public CombatMovesetObject movesetObject;

    /// <summary>
    /// The moveset class for the wepaon
    /// </summary>
    private CombatMoveset moveset;

    /// <summary>
    /// List of colliders hit with the weapon, used to deal damage once per move
    /// </summary>
    private List<Collider> hitEntities = new List<Collider>();

    #region Current Variables
    /// <summary>
    /// Index of which combo of the moveset we are on
    /// </summary>
    private int currentComboIndex;

    /// <summary>
    /// Index of which move of the combo we are on
    /// </summary>
    private int currentMoveIndex;

    /// <summary>
    /// The current CombatCombo we are on
    /// </summary>
    private CombatCombo currentCombo { get; set; }
    public CombatBindsEnum GetCurrentComboKey() {
        return currentCombo.type;
    }

    /// <summary>
    /// The current CombatMove we are on
    /// </summary>
    private CombatMove currentMove { get; set;}

    /// <summary>
    /// The current CombatAction we are on
    /// </summary>
    private CombatBaseObject currentAction { get; set; }
    public CombatBaseObject GetCurrentAction() {
        return currentAction;
    }

    #endregion

    #region VFX

    /// <summary>
    /// Which visual effect slash this action is associated with
    /// </summary>
    private VisualEffectAsset currentActionSlashVFX { get; set; }

    /// <summary>
    /// How long to wait to play the slash effect
    /// </summary>
    private float currentActionSlashDelay { get; set; }
    public float GetSlashDelay() {
        return currentActionSlashDelay;
    }

    /// <summary>
    /// The parent GameObject the slash is attached to on the weapon
    /// </summary>
    public GameObject slashObject;

    /// <summary>
    /// The visual effect component in the slash GameObject
    /// </summary>
    private VisualEffect slashEffect;
    
    /// <summary>
    /// Apply any rotation adjustments and play the slash effect currently loaded
    /// </summary>
    public void PlaySlashEffect() {

        // Apply the rotation offset from the current CombatMove
        Vector3 offset = currentMove.GetSlashRotation();
        if (offset != Vector3.zero){
            Quaternion newRotation = slashObject.transform.rotation;

            newRotation.x += offset.x;
            newRotation.y += offset.y;
            newRotation.z += offset.z;

            slashObject.transform.rotation = newRotation;
        }        

        slashEffect.Play();
    }
    #endregion

    #region Misc
    public CombatLink[] GetCombatLinks() {
        return currentCombo.combo[currentMoveIndex].linkableActions;
    }

    void Awake() {
        moveset = movesetObject.moveset;
        slashEffect = slashObject.GetComponentInChildren<VisualEffect>();
    }

    /// <summary>
    /// Jumps to specified MOVE of a specific COMBO via LINK data
    /// </summary>
    /// <param name="link">The CombatLink object containing jump information</param>
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

    /// <summary>
    /// Prepares the weapons internal data variables to the start of the combo associated with key
    /// </summary>
    /// <param name="key">The keycode entered</param>
    /// <returns>The combo associated with the keycode</returns>
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

    #endregion
}
