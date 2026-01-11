using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloneSkillController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private AnimEvents _animEvents;
    [SerializeField]
    private float _colorLoosingSpeed;
    private float _cloneTimer;
    [SerializeField]
    private Transform _attackCheck;
    [SerializeField]
    private float _attackCheckRadius = 0.8f;

    private Transform _closestEnemy;
    private float _facingDir;
    private float _chanceToDuplicate;
    private float _attackMultiplier;
    
    private int _enemyMask;
    [SerializeField]
    private float _closestEnemyCheckRadius = 25;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _animEvents = GetComponent<AnimEvents>();
        _enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        _cloneTimer -= Time.deltaTime;
        if (_cloneTimer < 0)
        {
            var alpha = _spriteRenderer.color.a - (Time.deltaTime * _colorLoosingSpeed);
            _spriteRenderer.color = new Color(1, 1, 1, alpha);
            
            if(alpha <= 0)
                ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.Clone, gameObject);
        }
    }

    public void SetupClone(Transform newTransform, bool canAttack, float duration, bool canDuplicateClone, float chanceDuplicate, Vector3 offset, Transform closeEnemy, float attackMultiplier)
    {
        if (canAttack)
            _animator.SetInteger("AttackNumber", Random.Range(1, 3));

        _attackMultiplier = attackMultiplier;
        _closestEnemy = closeEnemy;
        _chanceToDuplicate = chanceDuplicate;
        _facingDir = 1;
        _spriteRenderer.color = Color.white;
        transform.position = newTransform.position + offset;
        _cloneTimer = duration;

        _animEvents.HitCallBack((() =>
        {
            var colliders = Physics2D.OverlapCircleAll(_attackCheck.position, _attackCheckRadius);
            foreach (var hit in colliders)
            {
                var enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    var playerStats = PlayerManager.Instance.Player.Stats as PlayerStats;
                    enemy.SetupKnockbackDir(transform);
                    playerStats.CloneDoDamage(enemy, attackMultiplier);

                    if (SkillManager.Instance.Clone.CanApplyOnHitEffect)
                    {
                        var weaponData = InventoryManager.Instance.GetEquiped(EquipmentType.Weapon);
                        if(weaponData != null)
                            weaponData.OnEffect(enemy.transform);
                    }

                    if (canDuplicateClone)
                    {
                        if (Random.Range(0, 100) < _chanceToDuplicate)
                        {
                            SkillManager.Instance.Clone.CreateClone(enemy.transform, new Vector3(0.5f * _facingDir, 0));
                        }
                    }
                }
            }
        }));
        
        _animEvents.EndAnimCallBack((() =>
        {
            _cloneTimer = -1;
        }));

        StartCoroutine(FaceClosestTarget());
    }

    private IEnumerator FaceClosestTarget()
    {
        yield return null;

        FindClosestEnemy();
        
        if (_closestEnemy != null)
        {
            if (transform.position.x > _closestEnemy.position.x)
            {
                var flipDir = -1;
                _facingDir = flipDir;
                transform.localScale = new Vector3(flipDir, 1, 1);
            }
        }
    }

    private void FindClosestEnemy()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _closestEnemyCheckRadius, _enemyMask);
        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
            var distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                _closestEnemy = hit.transform;
            }
        }
    }
}
