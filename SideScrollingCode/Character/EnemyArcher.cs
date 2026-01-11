using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyArcher : Enemy
{
    [Header("Archer spesific info")]
    public float ArrowSpeed;
    public int ArrowDamage;
    public Vector2 _jumpVelocity;
    public float JumpCooldown;
    [HideInInspector]
    public float LastTimeJumped;
    public float JumpDistance;

    [FormerlySerializedAs("_groundBehindCheck")]
    [Header("collision check")]
    [SerializeField]
    private Transform _checkGroundBehind;
    [SerializeField]
    private Vector2 _groundBehindCheckSize;
    #region State
    public ArcherIdleState IdleState { get; private set; }
    public ArcherMoveState MoveState { get; private set; }
    public ArcherBattleState BattleState { get; private set; }
    public ArcherAttackState AttackState { get; private set; }
    public ArcherStunnedState StunnedState { get; private set; }
    public AtcherJumpState JumpState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        IdleState = new ArcherIdleState(this, StateMachine, "Idle", this);
        MoveState = new ArcherMoveState(this, StateMachine, "Move", this);
        BattleState = new ArcherBattleState(this, StateMachine, "Idle", this);
        AttackState = new ArcherAttackState(this, StateMachine, "Attack", this);
        StunnedState = new ArcherStunnedState(this, StateMachine, "Stunned", this);
        JumpState = new AtcherJumpState(this, StateMachine, "Jump", this);
        DeadState = new ArcherDeadState(this, StateMachine, "Idle");
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

    public bool CheckGroundBehind()
    {
        _checkGroundBehind.localScale = new Vector3(1 * _flipDir, 1, 1);
        return Physics2D.BoxCast(_checkGroundBehind.position, _groundBehindCheckSize, 0, Vector2.zero, 0, _groundMask);
    }

    public bool CheckWallBehind()
    {
        return Physics2D.Raycast(_checkWall.position, Vector2.right, (_wallCheckDistance + 1) * -FlipDir, _groundMask);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireCube(_checkGroundBehind.position, _groundBehindCheckSize);
    }
}
