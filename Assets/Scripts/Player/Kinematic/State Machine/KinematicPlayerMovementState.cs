using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KinematicPlayerMovementState : KinematicPlayerState
{
    public KinematicPlayerMovementState(KinematicPlayer player, KinematicPlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public abstract Vector3 CalculatePositionToMoveTo(Vector3 projectedPosition, Vector3 directionToMove, float speed);
}
