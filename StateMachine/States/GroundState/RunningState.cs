using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : GroundState
{
    float walkAcceleration;
    float groundDeceleration;

    public RunningState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
        walkAcceleration = character.WalkAcceleration;
        groundDeceleration = character.GroundDeceleration;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        float acceleration = walkAcceleration;
        float deceleration = groundDeceleration;

        if (character.MoveInput != 0)
        {
            character.Velocity.x = Mathf.MoveTowards(character.Velocity.x, speed * character.MoveInput, acceleration * Time.deltaTime);
        }
        else
        {
            character.Velocity.x = Mathf.MoveTowards(character.Velocity.x, 0, deceleration * Time.deltaTime);
        }

        character.MovePlayer(character.Velocity);

        Collider2D[] hits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, obstacleLayer);

        foreach (Collider2D hit in hits)
        {
            ColliderDistance2D colliderDistance = hit.Distance(character.PlayerCollider);

            if (colliderDistance.isOverlapped)
            {
                if (character.IsBumpRightWall(obstacleLayer) || character.IsBumpLeftWall(obstacleLayer))
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (character.Velocity.x == 0)
            stateMachine.ChangeState(character.Standing);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
