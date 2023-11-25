using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    // Classes (specific enemy states) that derive this state will have these enemy and enemyStateMachine variables behave as 'public'
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    // Constructor to pass in a reference to the enemy its attached to and its stateMachine code
    public EnemyState(Enemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    // Virtual methods are used when we want to override a certain behavior for the dervied class
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType animType) { }
}
