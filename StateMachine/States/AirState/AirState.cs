using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    float airAcceleration;
    float airDeceleration;

    public AirState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
        airAcceleration = character.AirAcceleration;
        airDeceleration = character.GroundDeceleration;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        float acceleration = airAcceleration;
        float deceleration = airDeceleration;

        if (character.MoveInput != 0)
        {
            character.Velocity.x = Mathf.MoveTowards(character.Velocity.x, speed * character.MoveInput, acceleration * Time.deltaTime);
        }
        else
        {
            character.Velocity.x = Mathf.MoveTowards(character.Velocity.x, 0, deceleration * Time.deltaTime);
        }
        character.Velocity.y += Physics2D.gravity.y * character.Mass * Time.deltaTime;
        character.MovePlayer(character.Velocity);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
