using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : Character
{
    [Header("Drop item")]
    [SerializeField]
    private int _equipmentDropChance;
    [SerializeField]
    private int _itemDropChance;
    #region State
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJump WallJumpState { get; private set; }
    public PlayerPrimaryAttackState AttackState { get; private set; }
    public PlayerCounterAttackState CounterAttackState { get; private set; }
    public PlayerAimSwordState AimSwordState { get; private set; }
    public PlayerCatchSwordState CatchSwordState { get; private set; }
    public PlayerBlackholeState BlackholeState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    #endregion

    [Header("Move Info")]
    [SerializeField]
    private float _jumpForce;
    public float JumpForce { get { return _jumpForce; } }
    [SerializeField]
    private float _dashSpeed = 25;
    public float DashSpeed { get { return _dashSpeed; } }
    private float _defaultMoveSpeed;
    private float _defaultJumpForce;
    private float _defaultDashSpeed;

    [Header("Attack Info")]
    [SerializeField]
    private Vector2[] _attackMovement;
    public Vector2[] AttackMovement
    {
        get { return _attackMovement; }
    }

    [Header("ThrowSword")]
    [SerializeField]
    private float _swordReturnImpact;
    public float SwordReturnImpact
    {
        get { return _swordReturnImpact; }
    }
    public float CounterAttackDuration { get; private set; }

    // Dash Info
    public float DashDir { get; private set; }

    public bool IsBusy { get; private set; }
    
    public GameObject Sword { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CounterAttackDuration = 0.2f;
        
        StateMachine = gameObject.AddComponent<PlayerStateMachine>();
        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState = new PlayerAirState(this, StateMachine, "Jump");
        DashState = new PlayerDashState(this, StateMachine, "Dash");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "WallSlide");
        WallJumpState = new PlayerWallJump(this, StateMachine, "Jump");
        AttackState = new PlayerPrimaryAttackState(this, StateMachine, "Attack");
        CounterAttackState = new PlayerCounterAttackState(this, StateMachine, "CounterAttack");
        AimSwordState = new PlayerAimSwordState(this, StateMachine, "AimSword");
        CatchSwordState = new PlayerCatchSwordState(this, StateMachine, "CatchSword");
        BlackholeState = new PlayerBlackholeState(this, StateMachine, "Jump");
        DeadState = new PlayerDeadState(this, StateMachine, "Die");

        if (_checkWall == null)
            _checkWall = transform;
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(IdleState);

        _defaultMoveSpeed = _moveSpeed;
        _defaultJumpForce = _jumpForce;
        _defaultDashSpeed = _dashSpeed;
    }

    protected override void Update()
    {
        if(Time.timeScale == 0)
            return;

        base.Update();
        StateMachine.CurrentState.Update();
        OnDash();

        if (Input.GetKeyDown(KeyCode.F) && SkillManager.Instance.CrystalSkill.CrystalUnlocked)
            SkillManager.Instance.CrystalSkill.CanUseSkill();
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            InventoryManager.Instance.UseFlask();
    }

    public override void SlowCharacterBy(float slowPercentage, float slowDuration)
    {
        base.SlowCharacterBy(slowPercentage, slowDuration);
        _moveSpeed = _moveSpeed * (1 - slowPercentage);
        _jumpForce = _jumpForce * (1 - slowPercentage);
        _dashSpeed = _dashSpeed * (1 - slowPercentage);
        Animator.speed = Animator.speed * (1 - slowPercentage);
        
        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        _moveSpeed = _defaultMoveSpeed;
        _jumpForce = _defaultJumpForce;
        _dashSpeed = _defaultDashSpeed;
    }

    public override void Die()
    {
        base.Die();
        StateMachine.ChangeState(DeadState);
        GameManager.Instance.LostCurrencyAmount = PlayerManager.Instance.Currency;
        PlayerManager.Instance.UpdateCurrency(-PlayerManager.Instance.Currency);
    }

    protected override void GenerateDrop()
    {
        var inventory = InventoryManager.Instance;
        var dropEquipList = new List<InventoryItem>();
        var dropItemList = new List<InventoryItem>();
        
        foreach (var item in inventory.GetEquipedList())
        {
            if (Random.Range(0, 100) <= _equipmentDropChance)
            {
                dropEquipList.Add(item);
                DropItem(item.Data);
            }
        }

        foreach (var item in dropEquipList)
        {
            inventory.UnequipItem(item.Data as EquipmentItemData);
        }

        foreach (var item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= _itemDropChance)
            {
                dropItemList.Add(item);
                DropItem(item.Data);
            }
        }

        foreach (var item in dropItemList)
        {
            inventory.RemoveItem(item.Data);
        }
    }

    public void SetNewSword(GameObject obj)
    {
        Sword = obj;
    }

    public void CatchSword()
    {
        StateMachine.ChangeState(CatchSwordState);
        Destroy(Sword);
    }

    private void OnDash()
    {
        if(CheckWall())
            return;
        
        if(!SkillManager.Instance.Dash.DashUnlocked)
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.Dash.CanUseSkill())
        {
            DashDir = Input.GetAxisRaw("Horizontal");
            if (DashDir == 0)
                DashDir = _flipDir;
                
            StateMachine.ChangeState(DashState);
        }
    }

    public IEnumerator OnBusyFor(float second)
    {
        IsBusy = true;

        yield return new WaitForSeconds(second);

        IsBusy = false;
    }
}
