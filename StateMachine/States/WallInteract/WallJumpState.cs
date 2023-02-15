using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : State
{
    float currentWallJumpTime = 0;

    public WallJumpState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Vector2 wallJumpDir = new Vector2();

        wallJumpDir.x = character.Direction > 0 ? -character.WallJumpPower.x : character.WallJumpPower.x;
        wallJumpDir.y = character.WallJumpPower.y;

        character.Velocity = wallJumpDir;

        character.MovePlayer(character.Velocity);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Collider2D[] hits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, obstacleLayer);

        character.MovePlayer(character.Velocity);

        foreach (Collider2D hit in hits)
        {
            ColliderDistance2D colliderDistance = hit.Distance(character.PlayerCollider);

            if (colliderDistance.isOverlapped)
            {
                if (character.IsBumpUpWall(obstacleLayer))
                {
                    character.Velocity.y = 0;
                }
                else if (character.IsBumpRightWall(obstacleLayer) || character.IsBumpLeftWall(obstacleLayer))
                {
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                }
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        currentWallJumpTime += Time.deltaTime;

        if (currentWallJumpTime > character.WallJUmpTime)
        {
            stateMachine.ChangeState(character.Falling);
        }
            
    }

    public override void Exit()
    {
        base.Exit();
        currentWallJumpTime = 0;
    }
}
