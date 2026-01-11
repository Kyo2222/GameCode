using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyDeathBringer : Enemy
{
    [HideInInspector]
    public bool BossFightBegun;
    [HideInInspector]
    public float LastTimeCast;
    [SerializeField]
    private float _spellStateCooldown;
    [SerializeField]
    private Vector2 _spellOffset;
    public int AmountOfSpells;
    public float SpellCooldown;
    
    [Header("Teleport details")]
    [SerializeField]
    private BoxCollider2D _arena;
    [SerializeField]
    private Vector2 _surroundingCheckSize;
    [HideInInspector]
    public float ChanceToTeleport;
    public float DeafultChanceToTeleport = 25;
    
    #region State
    public DeathBringerIdleState IdleState { get; private set; }
    public DeathBringerSpellCastState SpellCastState { get; private set; }
    public DeathBringerBattleState BattleState { get; private set; }
    public DeathBringerAttackState AttackState { get; private set; }
    public DeathBringerTeleportState TeleportState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        IdleState = new DeathBringerIdleState(this, StateMachine, "Idle", this);
        SpellCastState = new DeathBringerSpellCastState(this, StateMachine, "SpellCast", this);
        BattleState = new DeathBringerBattleState(this, StateMachine, "Move", this);
        AttackState = new DeathBringerAttackState(this, StateMachine, "Attack", this);
        TeleportState = new DeathBringerTeleportState(this, StateMachine, "Teleport", this);
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
            StateMachine.ChangeState(TeleportState);
            return true;
        }

        return false;
    }

    public void FindPosition()
    {
        var x = Random.Range(_arena.bounds.min.x + 3, _arena.bounds.max.x - 3);
        var y = Random.Range(_arena.bounds.min.y + 3, _arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x,
            transform.position.y - GroundBelow().distance + (Collider2D.size.y / 2));

        if (!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Looking for new position");
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 100, _groundMask);
    }

    private bool SomethingIsAround()
    {
        return Physics2D.BoxCast(transform.position, _surroundingCheckSize, 0, Vector2.zero, 0, _groundMask);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, _surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= ChanceToTeleport)
        {
            ChanceToTeleport = DeafultChanceToTeleport;
            return true;
        }

        return false;
    }

    public void CastSpell()
    {
        var player = PlayerManager.Instance.Player;
        float xOffset = 0;
        if (player.Rigidbody2D.velocity.x != 0)
            xOffset = player.FlipDir * _spellOffset.x;
        
        var spellPosition =
            new Vector3(player.transform.position.x + xOffset, player.transform.position.y + _spellOffset.y);

        var newSpell = ObjPoolManager.Instance.SpellCastPool.Get(spellPosition, quaternion.identity);
        newSpell.GetComponent<DeathBringerSpellController>().SetupSpell(Stats);
    }

    public bool CanDoSpellCast()
    {
        return Time.time >= LastTimeCast + _spellStateCooldown;
    }
}
