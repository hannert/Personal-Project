using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemies will have a kinematic rigidbody
public interface IMovable 
{
    Rigidbody enemyRb { get; set; }

    void moveEnemy(Vector3 positionToMoveTo);


}
