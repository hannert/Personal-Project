using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementState : PlayerState
{
    public PlayerMovementState(Player player, PlayerStateMachine playerStateMachine, string name) : base(player, playerStateMachine, name)
    {
    }

    public abstract void AddForceToRB(Vector3 acceleration);

    public virtual void ProcessInput() { }
}
