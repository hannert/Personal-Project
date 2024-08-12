
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public enum CombatBindsEnum
{
    PRIMARY_FIRE,
    SECONDARY_FIRE,
    PRIMARY_WEAPON_SKILL,
    SECONDARY_WEAPON_SKILL,

}

public enum PlayerStateReq
{
    IDLE,
    WALKING,
    SPRINTING,
    ROLLING,
    
}

public class CombatBinds {
    
    public static readonly Dictionary<KeyCode, CombatBindsEnum> codeToEnum = new() {
        { Keybinds.primaryFire, CombatBindsEnum.PRIMARY_FIRE },
        { Keybinds.secondaryFire, CombatBindsEnum.SECONDARY_FIRE },
        { Keybinds.primarySkill, CombatBindsEnum.PRIMARY_WEAPON_SKILL }
    };

    public static readonly Dictionary<CombatBindsEnum, KeyCode> enumToCode = codeToEnum.ToDictionary(x => x.Value, x => x.Key);


}