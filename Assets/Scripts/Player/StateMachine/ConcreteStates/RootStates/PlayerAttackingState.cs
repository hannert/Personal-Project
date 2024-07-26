using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerCombatState
{
    private GameObject weapon;
    private CombatBaseObject[] moveset;

    private int currentMoveIndex;

    private CombatBaseObject currentMove;

    public PlayerAttackingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name, GameObject weapon) : base(playerStateFactory, playerStateMachine, name)
    {
        _isRootState = true;
        this.weapon = weapon;
    }
    
    // Should get the weapons moveset
    public override void EnterState()
    {
        Logging.logState("<color=green>Entered</color> <color=brown>grounded</color> State");
        _ctx.isAttacking = true;

        currentMoveIndex = 0;
        
    }

    // Exit when the combo is done or timer has ran out to continue the combo
    public override void ExitState()
    {
        Logging.logState("<color=red>Exited</color> <color=brown>grounded</color> State");
        _ctx.isAttacking = false;
        
    }

    private void NextMove()
    {
        currentMoveIndex++;
        currentMove = moveset[currentMoveIndex];
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            // Go to the next move if applicable or exit
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Update timers in here 

    }


}
