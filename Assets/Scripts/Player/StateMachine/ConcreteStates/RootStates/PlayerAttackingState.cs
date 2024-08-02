using UnityEngine;

public class PlayerAttackingState : PlayerCombatState
{
    private GameObject weaponObject;
    private WeaponBase weapon;

    // Time that has passed since the move was triggered
    private float timeElapsed = 0;
    private float maxLinkTime;
    private float minLinkTime;

    /// <summary>
    /// The initial input when this state is entered
    /// </summary>
    private KeyCode initInput;

    /// <summary>
    /// The current move
    /// </summary>
    private CombatBaseObject currentMove;

    /// <summary>
    /// The links available to the current move
    /// </summary>
    private CombatLink[] currentMoveLinks;

    /// <summary>
    /// The key to advance in the combo
    /// </summary>
    private CombatBindsEnum currentComboKey;

    public PlayerAttackingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name, GameObject weapon, KeyCode initInput) : base(playerStateFactory, playerStateMachine, name)
    {
        _isRootState = true;
        this.weaponObject = weapon;
        this.weapon = weapon.GetComponent<WeaponBase>();
        this.initInput = initInput;
        //this.moveset = this.weapon.GetMoveset();
    }
    
    // Should get the weapons moveset
    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=red>Attacking</color> State");
        _ctx.isAttacking = true;

        //weapon.ResetCombo();
        weapon.Prepare(initInput);


        currentMove = weapon.GetCurrentMove();
        currentMoveLinks = weapon.GetCombatLinks(); 

        maxLinkTime = currentMove.maxLinkTime;
        minLinkTime = currentMove.minLinkTime;
        _ctx.SetAttackAnimation(currentMove.animation);
        _ctx.PlayAttackAnimation();
    }

    // Exit when the combo is done or timer has ran out to continue the combo
    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=red>Attacking</color> State");
        _ctx.isAttacking = false;
        
    }

    public override bool CheckSwitchStates()
    {
        return base.CheckSwitchStates();
    }

    private bool NextMove()
    {
        // Advance and check if end of moveset array, return
        if (weapon.NextMove() == true) {
            return false;
        }

        NewMove();
        return true;
    }

    private void NextCombo(CombatLink link)
    {
        weapon.LinkToNewCombo(link);
        NewMove();
        currentComboKey = weapon.GetCurrentComboKey();
    }

    private void NewMove() {
        currentMove = weapon.GetCurrentMove();
        currentMoveLinks = weapon.GetCombatLinks();

        timeElapsed = 0;

        maxLinkTime = currentMove.maxLinkTime;
        minLinkTime = currentMove.minLinkTime;

        _ctx.SetAttackAnimation(currentMove.animation);
        _ctx.PlayAttackAnimation();
    }

    public override void FrameUpdate()
    {

        timeElapsed += Time.deltaTime;

        // If the key pressed is the same as the current combos key, advance the combo
        if (Input.GetKeyDown(CombatBinds.enumToCode[currentComboKey])) {
            // Go to the next move if applicable or exit
            if (timeElapsed >= minLinkTime && timeElapsed <= maxLinkTime) {
                if (NextMove() == true) {
                    return;
                }
            }
        }

        // If key pressed is in the linkableActions, switch to that combo
        if (currentMoveLinks != null) {
            if (timeElapsed >= minLinkTime && timeElapsed <= maxLinkTime) {
                CombatBindsEnum tempBind;
                for (int i = 0 ; i < currentMoveLinks.Length; i++) {
                    tempBind = currentMoveLinks[i].inputToLink;
                    if (Input.GetKeyDown(CombatBinds.enumToCode[tempBind])) {
                        NextCombo(currentMoveLinks[i]);
                    }
                }
            }
        }


        if (timeElapsed >= maxLinkTime) {
            SwitchState(_factory.Grounded());
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }


}
