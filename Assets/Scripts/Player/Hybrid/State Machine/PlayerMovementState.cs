using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementState : PlayerState
{
    public PlayerMovementState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public abstract Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed);
}
