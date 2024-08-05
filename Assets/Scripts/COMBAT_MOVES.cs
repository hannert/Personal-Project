using System;
using UnityEngine;
using UnityEngine.VFX;

/// The file containing all classes related to a weapons' moves


/// <summary>
/// The lowest tier of data for a player COMBO, which consists of MOVES
/// </summary>
[System.Serializable]
public class CombatMove
{

    public CombatBaseObject action;

    public VisualEffectAsset slashAsset;

    public float slashDelay;

    public Vector3 slashRotationOffset;

    public Vector3 GetSlashRotation() {
        return slashRotationOffset;
    }

    public CombatLink[] linkableActions;
}

/// <summary>
/// Information on which COMBO to link TO and MOVE to start FROM
/// </summary>
[System.Serializable]
public class CombatLink
{
    public int comboIndex;

    public int moveIndex;

    public CombatBindsEnum inputToLink;

}


/// <summary>
/// A players COMBO, consisting of MOVES, also holds which COMBAT BIND the COMBO is associated with
/// </summary>
[System.Serializable]
public class CombatCombo
{
    public CombatMove[] combo;
    public CombatBindsEnum type;
}

/// <summary>
/// The overarching class to hold all of the players COMBOS
/// </summary>
[System.Serializable]
public class CombatMoveset
{
    public CombatCombo[] moves;

    /// <summary>
    /// Retrieve the CombatMove array associated with the keybind for this moveset
    /// </summary>
    /// <param name="comboInput">The KeyCode associated with the input</param>
    /// <returns>
    ///  The CombatMove array associated with the input <br/>
    ///  <b>null</b> if not found
    /// </returns>
    public CombatCombo GetCombo(KeyCode comboInput) {
        CombatBindsEnum combatBind;

        if (CombatBinds.codeToEnum.TryGetValue(comboInput, out combatBind) == false){
            return null;
        }

        for (int i = 0; i < moves.Length; i++){
            if (moves[i].type == combatBind){
                return moves[i];
            }
        }

        return null;
    }


}