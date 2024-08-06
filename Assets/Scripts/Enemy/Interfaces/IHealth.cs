using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHealth 
{
    // Make skeleton of what being able to be damaged entails
    void Damage(float damage);

    void Die();

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }


}
