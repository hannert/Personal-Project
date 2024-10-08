using System;
using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; set; }

    public Rigidbody enemyRb { get; set; }

    public Collider hurtBox;
    public Animator enemyAnimator;
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
        
        CurrentHealth = MaxHealth;
    }


    public void Start()
    {

        enemyRb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
        hurtBox = GetComponent<Collider>();
        stateMachine.Initialize(enemyIdleState);
    }

    #region IDamagable


    public void Damage(float damage)
    {   
        enemyAnimator.Play("Base Layer.Hit");
        
        
        CurrentHealth -= damage;
        Debug.Log(CurrentHealth.ToString());
        Signals.Get<EnemyDamage>().Dispatch();

        if (CurrentHealth < 0f )
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
            if (weapon.AddToHit(hurtBox) == true) {
                Damage(weapon.BaseDamage);
        } 
            
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
