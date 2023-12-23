using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPlayerAttackState : KinematicPlayerState
{

    // How long this current attack should take, should be different for each 'move' 
    public float durationOfAttack;

    // How much time has elapsed since entering this attack
    public float timeSpentAttacking;

    public KinematicPlayerAttackState(KinematicPlayer player, KinematicPlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override bool CheckSwitchStates()
    {
        if (timeSpentAttacking > durationOfAttack)
        {
            SwitchState(player.playerIdleWeaponState);
            return true;
        }


        return false;
    }

    public override void EnterState()
    {
        _psm.isAttacking = true;
        _psm.playerAnim.SetBool("isAttacking", true);
        timeSpentAttacking = 0;
        durationOfAttack = 1;
        Debug.Log("Enter attack");
    }

    public override void ExitState()
    {
        _psm.isAttacking = false;
        _psm.playerAnim.SetBool("isAttacking", false);
        Debug.Log("Leave attack");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        var directionFacing = PlayerUtilities.getDirectionFromOrigin(_psm.playerRb.rotation.y);
        var endPosition = _psm.projectedPos + directionFacing * _psm.speed * 0.3f * Time.fixedDeltaTime;
        timeSpentAttacking += Time.fixedDeltaTime;
        _psm.projectedPos = endPosition;
        CheckSwitchStates();
    }
}
