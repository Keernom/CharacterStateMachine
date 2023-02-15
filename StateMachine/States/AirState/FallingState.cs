using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : AirState
{
    public FallingState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Collider2D[] hits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, obstacleLayer);

        foreach (Collider2D hit in hits)
        {
            ColliderDistance2D colliderDistance = hit.Distance(character.PlayerCollider);

            if (colliderDistance.isOverlapped)
            {
                if (character.IsBumpUpWall(obstacleLayer))
                {
                    character.Velocity.y = 0;
                }
                else if (character.IsBumpDownWall(obstacleLayer))
                {
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                    stateMachine.ChangeState(character.Running);
                }
                else if (character.IsBumpRightWall(obstacleLayer) || character.IsBumpLeftWall(obstacleLayer))
                {
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                    if (IsVerticalWall(colliderDistance))
                        stateMachine.ChangeState(character.Climbing);
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

        if (platHits.Length == 0)
            character.IgnorePlatform = new Collider2D();
    }

    bool IsVerticalWall(ColliderDistance2D distance)
    {
        float angleTollerance = 2f;
        float angleDifferecne = Mathf.Abs(90 - Vector2.Angle(distance.normal, Vector2.up));
        return angleTollerance >= angleDifferecne;
    }

    public override void Exit()
    {
        base.Exit();
        
    }
}
