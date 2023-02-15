using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : State
{
    public ClimbingState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        character.Velocity = Vector2.zero;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (character.Velocity.y != character.ClimbingGravity)
            character.Velocity.y = Mathf.MoveTowards(character.Velocity.y, character.ClimbingGravity, character.ClimbingDownSpeed * Time.deltaTime);

        character.MovePlayer(character.Velocity);

        Collider2D[] hits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, obstacleLayer);

        foreach (Collider2D hit in hits)
        {
            ColliderDistance2D colliderDistance = hit.Distance(character.PlayerCollider);

            if (!character.IsBumpRightWall(obstacleLayer) && !character.IsBumpLeftWall(obstacleLayer))
            {
                stateMachine.ChangeState(character.Falling);
            }

            if (colliderDistance.isOverlapped)
            {
                if (character.IsBumpRightWall(obstacleLayer) != character.MoveInput > 0)
                    stateMachine.ChangeState(character.Falling);

                if (character.IsBumpUpWall(obstacleLayer))
                {
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                }
                else if (character.IsBumpDownWall(obstacleLayer))
                {
                    stateMachine.ChangeState(character.Running);
                }
            }
        }

        Collider2D[] platHits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, platformLayer);

        foreach (Collider2D phit in platHits)
        {
            ColliderDistance2D colliderDistance = phit.Distance(character.PlayerCollider);

            if (colliderDistance.isOverlapped)
            {
                if (character.IsBumpDownWall(platformLayer) && phit != character.IgnorePlatform)
                {
                    stateMachine.ChangeState(character.Running);
                }
            }
        }
    }

    public override void LogicUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(character.WallJumping);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
