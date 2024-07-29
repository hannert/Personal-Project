using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerCombatState
{
    private GameObject weaponObject;
    private WeaponBase weapon;
    private CombatBaseObject[] moveset;

    private int currentMoveIndex;

    // Time that has passed since the move was triggered
    private float timeElapsed = 0;
    private float linkTime;

    private CombatBaseObject currentMove;

    public PlayerAttackingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name, GameObject weapon) : base(playerStateFactory, playerStateMachine, name)
    {
        _isRootState = true;
        this.weaponObject = weapon;
        this.weapon = weapon.GetComponent<WeaponBase>();
        this.moveset = this.weapon.GetMoveset();
    }
    
    // Should get the weapons moveset
    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=red>Attacking</color> State");
        _ctx.isAttacking = true;

        // Initialize the first move
        currentMoveIndex = 0;

        linkTime = moveset[0].linkTime;
        _ctx.SetAttackAnimation(moveset[0].animation);
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

    private void NextMove()
    {
        currentMoveIndex++;
        // End of moveset array, exit state
        if (currentMoveIndex >= moveset.Length) {
            return;
        }

        // Advance
        currentMove = moveset[currentMoveIndex];

        // Reset the time variable
        timeElapsed = 0;

        linkTime = currentMove.linkTime;
        
    }

    public override void FrameUpdate()
    {

        timeElapsed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            // Go to the next move if applicable or exit
            if (timeElapsed <= linkTime) {
                NextMove();
            }
        }
        if (timeElapsed >= linkTime) {
            Debug.Log("Time ran out for attack");
            SwitchState(_factory.Grounded());
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Update timers in here 

    }


}
