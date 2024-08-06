using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All enemies will employ the damage interface
//  > Enemy behavior script will have an IDamagable interface
// Damagable Interface: The enemies CAN be damaged 
// Interfaces need to be implemented in the classes where they are called upon, which is the HOW can this be damaged
public interface IDamagable 
{
    // Make skeleton of what being able to be damaged entails
    void Damage(float damage);

    void Die();

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
    

}
