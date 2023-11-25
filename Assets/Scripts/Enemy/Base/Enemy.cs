using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovable
{
    [field: SerializeField] public float maxHealth { get; set; } = 100f;
    public float currentHealth { get; set; }
    public Rigidbody enemyRb { get; set; }

    #region Enemy State Machine ---------

    public EnemyStateMachine stateMachine;

    public EnemyIdleState enemyIdleState;
    public EnemyChaseState enemyChaseState;
    public EnemyAttackState enemyAttackState;

    #endregion ---------------

    public void Awake()
    {
        stateMachine = new EnemyStateMachine();

        enemyIdleState = new EnemyIdleState(this, stateMachine);
        enemyAttackState = new EnemyAttackState(this, stateMachine);
        enemyChaseState = new EnemyChaseState(this, stateMachine);
    }


    public void Start()
    {
        currentHealth = maxHealth;

        enemyRb = GetComponent<Rigidbody>();

        stateMachine.Initialize(enemyIdleState);
    }

    #region IDamagable
    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0f )
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion


    #region IMovable
    public void moveEnemy(Vector3 positionToMoveTo)
    {
        throw new System.NotImplementedException();
    }


    #endregion


#region Animation 

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        stateMachine.currentEnemyState.AnimationTriggerEvent(triggerType);
    }
    public enum AnimationTriggerType
    {
        Idle,
        Chase
    }

#endregion

}
