using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementState : PlayerState
{
    public PlayerMovementState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }

    public abstract void AddForceToRB(Vector3 acceleration);

    public virtual void ProcessInput() { }
}
