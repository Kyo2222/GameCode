using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : Enemy
{
    public float BattleStateMoveSpeed;
    #region State
    public SkeletonIdleState IdleState { get; private set; }
    public SkeletonMoveState MoveState { get; private set; }
    public SkeletonBattleState BattleState { get; private set; }
    public SkeletonAttackState AttackState { get; private set; }
    public SkeletonStunnedState StunnedState { get; private set; }
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        IdleState = new SkeletonIdleState(this, StateMachine, "Idle", this);
        MoveState = new SkeletonMoveState(this, StateMachine, "Move", this);
        BattleState = new SkeletonBattleState(this, StateMachine, "Move", this);
        AttackState = new SkeletonAttackState(this, StateMachine, "Attack", this);
        StunnedState = new SkeletonStunnedState(this, StateMachine, "Stunned", this);
        DeadState = new SkeletonDeadState(this, StateMachine, "Idle");
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(IdleState);
    }
    
    public override bool CanStunned()
    {
        if (base.CanStunned())
        {
            StateMachine.ChangeState(StunnedState);
            return true;
        }

        return false;
    }
}
