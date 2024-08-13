using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
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
    [field: SerializeField]
    public float BaseDamage {get; private set; }

    /// <summary>
    /// The scriptable object holding the moveset for this weapon
    /// </summary>
    [field: SerializeField]
    public CombatMovesetObject MovesetObject { get; private set;}

    /// <summary>
    /// The moveset class for the weapon
    /// </summary>
    public CombatMoveset Moveset { get; private set; }

    /// <summary>
    /// List of colliders hit with the weapon, used to deal damage once per move
    /// </summary>
    private List<Collider> HitEntities { get; set; } = new List<Collider>(); 

    #region Current Variables
    /// <summary>
    /// Index of which combo of the moveset we are on
    /// </summary>
    private int CurrentComboIndex { get; set; }

    /// <summary>
    /// Index of which move of the combo we are on
    /// </summary>
    private int CurrentMoveIndex { get; set; }

    /// <summary>
    /// The current CombatCombo we are on
    /// </summary>
    private CombatCombo CurrentCombo { get; set; }
    public CombatBindsEnum GetCurrentComboKey() {
        return CurrentCombo.type;
    }

    /// <summary>
    /// The current CombatMove we are on
    /// </summary>
    private CombatMove CurrentMove { get; set; }

    /// <summary>
    /// The current CombatAction we are on
    /// </summary>
    public CombatBaseObject CurrentAction {  get; private set; }

    #endregion

    #region VFX

    /// <summary>
    /// Which visual effect slash this action is associated with
    /// </summary>
    private VisualEffectAsset CurrentActionSlashVFX { get; set; }

    /// <summary>
    /// How long to wait to play the slash effect
    /// </summary>
    public float CurrentActionSlashDelay { get; private set; }

    /// <summary>
    /// The parent GameObject the slash is attached to on the weapon
    /// </summary>
    [field: SerializeField]
    public GameObject slashObject { get; private set; }

    /// <summary>
    /// The visual effect component in the slash GameObject
    /// </summary>
    private VisualEffect slashEffect { get; set; }
    
    /// <summary>
    /// Apply any rotation adjustments and play the slash effect currently loaded
    /// </summary>
    public void PlaySlashEffect() {
        // Reset the rotation
        slashObject.transform.localPosition = Vector3.zero;
        slashObject.transform.localEulerAngles = Vector3.zero;

        // Apply the rotation offset from the current CombatMove
        Vector3 offset = CurrentMove.GetSlashRotation();
        if (offset != Vector3.zero){
            Vector3 newRotation = new Vector3();

            newRotation.x = offset.x;
            newRotation.y = offset.y;
            newRotation.z = offset.z;

            slashObject.transform.localEulerAngles = newRotation;
        }        
        
        // Play the effect
        slashEffect.Play();
    }
    #endregion

    #region Misc

    void Awake() {
        Moveset = MovesetObject.moveset;
        slashEffect = slashObject.GetComponentInChildren<VisualEffect>();
        Signals.Get<Player.DamageTick>().AddListener(ResetHitList);
    }


    public CombatLink[] GetCombatLinks() {
        return CurrentCombo.combo[CurrentMoveIndex].linkableActions;
    }

    public void ResetHitList() {
        Debug.Log("ResetHitList");
        HitEntities.Clear();
    }

    public List<CombatBindsEnum> GetComboEntryKeys(PlayerStateReq state) {
        List<CombatBindsEnum> returnList = new List<CombatBindsEnum>();

        for (int i = 0; i < Moveset.moves.Length; i++) {
            CombatCombo tempMove = Moveset.moves[i];
            for (int j = 0; j < tempMove.req.Length; j++){
                if (tempMove.req[j] == state){
                    returnList.Add(tempMove.type);
                }
            }
        }

        return returnList;
    }


    /// <summary>
    /// Jumps to specified MOVE of a specific COMBO via LINK data
    /// </summary>
    /// <param name="link">The CombatLink object containing jump information</param>
    /// <returns></returns>
    public void LinkToNewCombo(CombatLink link) {
        Debug.Log("Linking new combo" + link.comboIndex.ToString() + " " + link.moveIndex.ToString());
        // Clear hit colliders array since we are going to a new move
        HitEntities.Clear();

        CurrentComboIndex = link.comboIndex;
        CurrentMoveIndex = link.moveIndex;

        CurrentCombo = Moveset.moves[CurrentComboIndex];
        CurrentMove = CurrentCombo.combo[CurrentMoveIndex];
        CurrentAction = CurrentCombo.combo[CurrentMoveIndex].action;

        CurrentActionSlashVFX = CurrentMove.slashAsset;
        CurrentActionSlashDelay = CurrentMove.slashDelay;
    }

    /// <summary>
    /// Advances the current move in the current combo
    /// </summary>
    /// <returns></returns>
    public bool NextMove() {
        // If we reach the end of the combo string, return true
        if ((++CurrentMoveIndex) >= CurrentCombo.combo.Length) {
            return true;
        }

        // Clear hit colliders array since we are going to a new move
        HitEntities.Clear();
        
        // Advance to the next move in the combo
        CurrentMove = CurrentCombo.combo[CurrentMoveIndex];
        CurrentAction = CurrentCombo.combo[CurrentMoveIndex].action;
        
        return false;
    }

    /// <summary>
    /// Prepares the weapons internal data variables to the start of the combo associated with key
    /// </summary>
    /// <param name="key">The keycode entered</param>
    /// <returns>The combo associated with the keycode</returns>
    public bool Prepare(KeyCode key, PlayerStateReq state) {
        // Clear hit colliders array since we are going to a new move
        HitEntities.Clear();

        CurrentCombo = Moveset.GetCombo(key, state);
        CurrentMoveIndex = 0;
        CurrentMove = CurrentCombo.combo[CurrentMoveIndex];
        CurrentAction = CurrentCombo.combo[CurrentMoveIndex].action;
        
        CurrentActionSlashVFX = CurrentMove.slashAsset;
        CurrentActionSlashDelay = CurrentMove.slashDelay;

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
        if (HitEntities.Contains(hitEntity)){
            return false;
        } else {
            HitEntities.Add(hitEntity);
        }
        
        return true;
    }

    #endregion
}
