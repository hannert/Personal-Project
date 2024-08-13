using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmotingState : PlayerState
{
    public PlayerEmotingState(PlayerStateFactory playerStateFactory, PlayerStateMachine playerStateMachine, string name) : base(playerStateFactory, playerStateMachine, name)
    {
    }


    public override void EnterState()
    {
      _ctx.PlayerAnim.Play("wave");
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void PhysicsUpdate()
    {
    }



    
}
