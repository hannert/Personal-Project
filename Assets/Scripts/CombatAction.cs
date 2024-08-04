using System;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class CombatAction
{
    public CombatBaseObject combatObject;

}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CombatLink
{
    public int comboIndex;

    public int moveIndex;

    public CombatBindsEnum inputToLink;

}


/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CombatMove
{
    public CombatBaseObject action;

    public VisualEffectAsset slashAsset;

    public float slashDelay;

    public CombatLink[] linkableActions = new CombatLink[3];
}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CombatCombo
{
    public CombatMove[] combo;
    public CombatBindsEnum type;
}

/// <summary>
/// Awesome
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