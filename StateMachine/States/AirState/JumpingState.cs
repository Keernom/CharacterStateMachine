using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : AirState
{
    float jumpHeight;

    public JumpingState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
        jumpHeight = character.JumpHeight;
    }

    public override void Enter()
    {
        base.Enter();
        character.Velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (Input.GetKey(KeyCode.Space) && character.JumpIncreasingTime > character.CurrentJumpIncreasingTime)
        {
            character.CurrentJumpIncreasingTime += Time.deltaTime;
        }
        else
        {
            character.Velocity.y *= .7f;  // Decreasing vertical speed for fast speed changing after jump
        }

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

        if (character.Velocity.y < 0)
        {
            stateMachine.ChangeState(character.Falling);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
