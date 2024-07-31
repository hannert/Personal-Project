using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Instead of using interfaces, maybe we can use a components based system for drag and dropping
public class Health : MonoBehaviour
{
    
    public float MAX_HEALTH;

    public float currentHealth { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MAX_HEALTH;
    }


    public void Damage(float damage) {
        currentHealth -= damage;

        if (currentHealth >= 0) {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
