using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DamagedState : State
{
    Vector2 _damagedKnockback;
    float _XKnockbackDirection;

    float _duration;
    float _currentDuration;
    bool _grounded;

    public DamagedState(MovementStateMachine character, StateMachine stateMachine) : base(character, stateMachine)
    {
        _duration = character.DamagedStateDuration;
        _damagedKnockback = character.DamagedKnockback;
    }

    public override void Enter()
    {
        base.Enter();
        character.CurrentInvincibilityTime = 0;

        PlayerEventManager.OnTakeDamage?.Invoke(character.TakenDamage);

        character.Velocity.y = _damagedKnockback.y;
        _XKnockbackDirection = -character.Direction;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        _currentDuration += Time.deltaTime;
        if (_currentDuration >= _duration)
        {
            stateMachine.ChangeState(character.Falling);
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (!_grounded)
        {
            character.Velocity.y += Physics2D.gravity.y * character.Mass * Time.deltaTime;
            character.Velocity.x = _damagedKnockback.x * _XKnockbackDirection;
        }

        character.MovePlayer(character.Velocity);

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
                    character.Velocity.y = 0;
                    _grounded = true;
                }
                else if (character.IsBumpRightWall(obstacleLayer) || character.IsBumpLeftWall(obstacleLayer))
                {
                    character.transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
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

    public override void Exit()
    {
        base.Exit();
        _currentDuration = 0;
        _grounded = false;
        character.TakenDamage = 0;
    }
}
