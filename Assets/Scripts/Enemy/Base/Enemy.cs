using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovable
{
    [field: SerializeField] public float maxHealth { get; set; } = 100f;
    [field: SerializeField] public float currentHealth { get; set; }

    public Rigidbody enemyRb { get; set; }
    public class EnemyDamage : ASignal {}
    
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
        
        currentHealth = maxHealth;
    }


    public void Start()
    {

        enemyRb = GetComponent<Rigidbody>();

        stateMachine.Initialize(enemyIdleState);
    }

    #region IDamagable


    public void Damage(float damage)
    {
        currentHealth -= damage;
        Signals.Get<EnemyDamage>().Dispatch();

        if (currentHealth < 0f )
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        WeaponBase weapon = null;
        // Check if the other collider has the weapon component attached
        if ((weapon = other.gameObject.GetComponent<WeaponBase>()) != null){
            Damage(weapon.baseDamage);
        }
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
