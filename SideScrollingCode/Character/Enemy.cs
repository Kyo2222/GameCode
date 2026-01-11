using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(CharacterFX))]
public class Enemy : Character
{
    #region State
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyState DeadState { get; protected set; }
    #endregion
    
    protected LayerMask _playerMask;
    protected Slider _slider;
    [Header("Drop item")]
    [SerializeField]
    private int _maxItemToDrop;
    [SerializeField]
    private ItemData[] _dropItems;
    private List<ItemData> _dropList = new List<ItemData>();

    [Header("Time")]
    [SerializeField]
    private float _idleTime = 2;

    public float IdleTime { get { return _idleTime; } }
    private float _defaultMoveSpeed;

    [SerializeField]
    private float _battleTime = 7;
    public float BattleTime { get { return _battleTime; } }

    [Header("Attack Info")]
    public float _agroDistance = 2;
    [SerializeField]
    private float _attackDistance = 2;
    public float AttackDistance { get { return _attackDistance; } }
    [HideInInspector]
    public float AttackCooldown;
    public float MinAttackCooldown = 1;
    public float MaxAttackCooldown = 2;
    [HideInInspector]
    public float LastAttackedTime;

    private static int PlayerDistance = 8;

    [Header("Stunned Info")]
    [SerializeField]
    private float _stunDuration = 1;
    public float StunDuration
    {
        get { return _stunDuration; }
    }
    [SerializeField]
    private Vector2 _stunDirection = new Vector2(10, 12);
    public Vector2 StunDirection
    {
        get { return _stunDirection; }
    }
    protected bool _canStunned;
    [SerializeField]
    protected GameObject _counterImage;

    public string LastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = gameObject.AddComponent<EnemyStateMachine>();
        _playerMask = LayerMask.GetMask("Player");
        _defaultMoveSpeed = _moveSpeed;
        _slider = GetComponentInChildren<Slider>();
        _slider.maxValue = Stats.GetMaxHealthValue();
        Stats.OnUpdateHealth += UpdateHealth;
    }

    protected override void Start()
    {
        base.Start();
        UpdateHealth();
    }

    private void OnDestroy()
    {
        Stats.OnUpdateHealth -= UpdateHealth;
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.Update();
    }

    protected virtual void UpdateHealth()
    {
        _slider.value = Stats.CurrentHealth;
    }
    
    public override void OnFlip(bool isForward)
    {
        base.OnFlip(isForward);
        _slider.transform.localScale = new Vector3(FlipDir, 1, 1);
    }

    public override void SlowCharacterBy(float slowPercentage, float slowDuration)
    {
        base.SlowCharacterBy(slowPercentage, slowDuration);
        _moveSpeed = _moveSpeed * (1 - slowPercentage);
        Animator.speed = Animator.speed * (1 - slowPercentage);

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        _moveSpeed = _defaultMoveSpeed;
    }

    public override void Die()
    {
        base.Die();
        StateMachine.ChangeState(DeadState);
    }

    protected override void GenerateDrop()
    {
        if(_dropItems.Length == 0)
            return;

        foreach (var item in _dropItems)
        {
            if(item != null && Random.Range(0, 100) <= item.DropChance)
                _dropList.Add(item);
        }
        
        for (int i = 0; i < _maxItemToDrop; i++)
        {
            if (_dropList.Count > 0)
            {
                var randomIndex = Random.Range(0, _dropList.Count);
                var randomItem = _dropList[randomIndex];
                
                _dropList.Remove(randomItem);
                DropItem(randomItem);
            }
            
        }
    }

    public virtual void AssignLastAnimName(string name)
    {
        LastAnimBoolName = name;
    }

    public virtual void FreezeTime(bool timerFrozen)
    {
        if (timerFrozen)
        {
            _moveSpeed = 0;
            Animator.speed = 0;
        }
        else
        {
            _moveSpeed = _defaultMoveSpeed;
            Animator.speed = 1;
        }
    }

    public virtual void FreezeTime(float duration)
    {
        StartCoroutine(FreezeTimerFor(duration));
    }

    protected virtual IEnumerator FreezeTimerFor(float _sec)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_sec);
        FreezeTime(false);
    }

    public virtual void OpenCounterAttackWindow(bool value)
    {
        _canStunned = value;
        // debug
        //_counterImage.SetActive(value);
    }

    public virtual bool CanStunned()
    {
        if (_canStunned)
        {
            OpenCounterAttackWindow(false);
            return true;
        }

        return false;
    }
    
    public RaycastHit2D CheckPlayer()
    {
        var playerDetected = Physics2D.Raycast(_checkWall.position, Vector2.right * FlipDir, PlayerDistance, _playerMask);
        var wallDetected = Physics2D.Raycast(_checkWall.position, Vector2.right * FlipDir, PlayerDistance, _groundMask);
        if (wallDetected)
        {
            if (wallDetected.distance < playerDetected.distance)
                return default(RaycastHit2D);
        }

        return playerDetected;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + _attackDistance * FlipDir, transform.position.y));
    }
}
