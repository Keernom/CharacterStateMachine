using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : State
{
    public GroundState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        character.Velocity.y = 0;
        character.MovePlayer(character.Velocity);
        character.RestoreJumps();
        character.RestoreDash();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (!character.DownSideCheck(obstacleLayer) && !character.DownSideCheck(platformLayer))
        {
            character.JumpsRemain--;
            stateMachine.ChangeState(character.Falling);
        }

        if (character.RightSideCheck(platformLayer) || character.LeftSideCheck(platformLayer))
            character.MovePlayer(new Vector2(0, 5));

        Collider2D[] platHits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, platformLayer);

        if (platHits.Length > 0 && !character.DownSideCheck(obstacleLayer))
        {
            if (Input.GetKey(KeyCode.S))
            {
                character.IgnorePlatform = platHits[0];
                stateMachine.ChangeState(character.Falling);
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
