using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
    float dashPower;
    float dashTime;

    float currentDashTime = 0;

    public DashState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
        this.dashPower = character.DashPower;
        this.dashTime = character.DashTime;
    }

    public override void Enter()
    {
        base.Enter();

        character.Velocity.y = 0;
        character.Velocity.x = dashPower * character.Direction;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
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
        currentDashTime += Time.deltaTime;

        if (currentDashTime > dashTime)
            stateMachine.ChangeState(character.Falling);
    }

    public override void Exit()
    {
        base.Exit();
        currentDashTime = 0;
    }
}
