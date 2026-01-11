using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum SlimeType
{
    Big,
    Medium,
    Small
}

public class EnemySlime : Enemy
{
    #region State
    public SlimeIdleState IdleState { get; private set; }
    public SlimeMoveState MoveState { get; private set; }
    public SlimeBattleState BattleState { get; private set; }
    public SlimeAttackState AttackState { get; private set; }
    public SlimeStunnedState StunnedState { get; private set; }
    #endregion

    [Header("Slime spesific")]
    [SerializeField]
    private SlimeType _type;
    private GameObject _parent;
    [SerializeField]
    private int _slimesToCreate;
    private GameObjectPool _objPool;
    [SerializeField]
    private List<GameObject> _slimePrefabList;
    [SerializeField]
    private Vector2 _minCreationVelocity;
    [SerializeField]
    private Vector2 _maxCreationVelocity;
    
    protected override void Awake()
    {
        base.Awake();
        IdleState = new SlimeIdleState(this, StateMachine, "Idle", this);
        MoveState = new SlimeMoveState(this, StateMachine, "Move", this);
        BattleState = new SlimeBattleState(this, StateMachine, "Move", this);
        AttackState = new SlimeAttackState(this, StateMachine, "Attack", this);
        StunnedState = new SlimeStunnedState(this, StateMachine, "Stunned", this);
        DeadState = new SlimeDeadState(this, StateMachine, "Idle");
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(IdleState);

        if (_type == SlimeType.Small)
            return;
        
        _parent = GameObject.Find("EnemyList");
        _objPool = new GameObjectPool(_slimePrefabList[(int) _type + 1], _parent);
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

    public override void OnFlip(bool isForward)
    {
        var currentScale = transform.localScale;
        _flipDir = isForward ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * _flipDir, currentScale.y, currentScale.z);
        _slider.transform.localScale = new Vector3(FlipDir, 1, 1);
    }

    public override void Die()
    {
        base.Die();
        
        if(_type == SlimeType.Small)
            return;
        
        CreateSlimes(_slimesToCreate);
    }

    private void CreateSlimes(int amountOfSlime)
    {
        for (int i = 0; i < amountOfSlime; i++)
        {
            var newSlime = _objPool.Get(transform.position, quaternion.identity);
            newSlime.GetComponent<EnemySlime>().SetupSlime();
        }
    }

    public void SetupSlime()
    {
        var xVelocity = Random.Range(_minCreationVelocity.x, _maxCreationVelocity.x);
        var yVelocity = Random.Range(_minCreationVelocity.y, _maxCreationVelocity.y);

        _isKnocked = true;
        Rigidbody2D.velocity = new Vector2(xVelocity * -FlipDir, yVelocity);

        DOVirtual.DelayedCall(1.5f, () =>
        {
            _isKnocked = false;
        });
    }
}
