using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine stateMachine;
    protected MovementStateMachine character;

    protected LayerMask obstacleLayer;
    protected LayerMask platformLayer;
    protected LayerMask damagableLayer;

    protected float speed;

    public State(MovementStateMachine character, StateMachine stateMachine)
    {
        this.character = character;
        this.stateMachine = stateMachine;

        obstacleLayer = character.ObstacleLayer;
        platformLayer = character.PlatformLayer;
        damagableLayer = character.DamagableLayer;

        speed = character.Speed;
    }

    public virtual void Enter()
    {
        character.CurrentJumpIncreasingTime = 0;
    }

    public virtual void StateUpdate()
    {
        Collider2D[] damagableHits = Physics2D.OverlapBoxAll(character.PlayerTransform.position, character.PlayerCollider.size, 0, damagableLayer);

        if (damagableHits.Length > 0 && (character.CurrentInvincibilityTime > character.InvincibilityTime))
        {
            character.TakenDamage = damagableHits[0].GetComponent<IDamagable>().GetDamage();
            stateMachine.ChangeState(character.Damaged);
        }

        character.CurrentInvincibilityTime += Time.deltaTime;
    }

    public virtual void LogicUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && character.JumpsRemain > 0 && stateMachine.CurrentState != character.Damaged)
        {
            stateMachine.ChangeState(character.Jumping);
            character.JumpsRemain--;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && character.DashRemain > 0 && stateMachine.CurrentState != character.Damaged)
        {
            stateMachine.ChangeState(character.Dashing);
            character.DashRemain--;
        }
    }
 

    public virtual void Exit()
    {

    }
}
