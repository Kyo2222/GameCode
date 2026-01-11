using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShady : Enemy
{
    [Header("Shady spesifics")]
    public float BattleStateMoveSpeed;
    public float GrowSpeed;
    public float MaxSize;

    #region State
    public ShadyIdleState IdleState { get; private set; }
    public ShadyMoveState MoveState { get; private set; }
    public ShadyBattleState BattleState { get; private set; }
    public ShadyStunnedState StunnedState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        IdleState = new ShadyIdleState(this, StateMachine, "Idle", this);
        MoveState = new ShadyMoveState(this, StateMachine, "Move", this);
        BattleState = new ShadyBattleState(this, StateMachine, "MoveFast", this);
        StunnedState = new ShadyStunnedState(this, StateMachine, "Stunned", this);
        DeadState = new ShadyDeadState(this, StateMachine, "Dead", this);
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
