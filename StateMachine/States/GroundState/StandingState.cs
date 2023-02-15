using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingState : GroundState
{
    public StandingState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (character.MoveInput != 0)
            stateMachine.ChangeState(character.Running);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
