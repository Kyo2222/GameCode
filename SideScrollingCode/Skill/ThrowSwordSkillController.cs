using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSwordSkillController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    private bool _canRotate;
    private bool _isReturning;
    
    private float _returnSpeed = 1;
    private float _freezeTimeDuration;

    private SwordType _swordType;
    private int _amount;
    private float _speed;
    private List<Transform> _enemyTarget = new List<Transform>();
    private int _targetIndex;

    // Spin Info
    private float _maxTravelDistance;
    private float _spinDuration;
    private float _spinTimer;
    private bool _isStopped;
    private float _hitTimer;
    private float _hitCooldown;
    private float _spinDirection;

    private ParticleSystem _fx;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _fx = GetComponentInChildren<ParticleSystem>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 dir, float gravityScale, float freezeTimeDuration, float returnSpeed)
    {
        _rb.velocity = dir;
        _rb.gravityScale = gravityScale;
        _freezeTimeDuration = freezeTimeDuration;
        _returnSpeed = returnSpeed;
        if(_swordType != SwordType.Pierce)
        {
            _canRotate = true;
            _animator.SetBool("Rotation", true);
        }
        
        Invoke("CatchSword", 7f);
    }

    private void CatchSword()
    {
        PlayerManager.Instance.Player.CatchSword();
    }

    public void SetupBounce(SwordType type, int amount, float speed)
    {
        _swordType = type;
        _amount = amount;
        _speed = speed;
    }

    public void SetupPierce(SwordType type, int amount)
    {
        _swordType = type;
        _amount = amount;
    }

    public void SetupSpin(SwordType type, float maxTravelDistance, float spinDuration, float cooldown)
    {
        _swordType = type;
        _maxTravelDistance = maxTravelDistance;
        _spinDuration = spinDuration;
        _hitCooldown = cooldown;
        _spinDirection = Mathf.Clamp(_rb.velocity.x, -1, 1);
    }

    public void ReturnSword()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        _isReturning = true;
    }

    private void Update()
    {
        if(_canRotate)
            transform.right = _rb.velocity;

        if (_isReturning)
        {
            var player = PlayerManager.Instance.Player;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, _returnSpeed);

            var dis = Vector2.Distance(transform.position, player.transform.position);
            if (dis < 1)
            {
                CatchSword();
            }
        }

        BounceSword();

        SpinSword();
    }

    private void StopWhenSpinning()
    {
        _isStopped = true;
        _rb.constraints = RigidbodyConstraints2D.FreezePosition;
        _spinTimer = _spinDuration;
    }

    private void BounceSword()
    {
        if (_swordType == SwordType.Bounce && _enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, _enemyTarget[_targetIndex].position,
                _speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _enemyTarget[_targetIndex].position) < 0.1f)
            {
                var enemy = _enemyTarget[_targetIndex].GetComponent<Enemy>();
                SkillDamage(enemy);
                _targetIndex++;
                _amount--;

                if (_amount <= 0)
                {
                    _swordType = SwordType.Normal;
                    _isReturning = true;
                }

                if (_targetIndex >= _enemyTarget.Count)
                    _targetIndex = 0;
            }
        }
    }

    private void SpinSword()
    {
        if (_swordType != SwordType.Spin)
            return;
        
        var player = PlayerManager.Instance.Player;
        if (Vector2.Distance(player.transform.position, transform.position) > _maxTravelDistance && !_isStopped)
        {
            StopWhenSpinning();
        }

        if (_isStopped)
        {
            _spinTimer -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(transform.position.x + _spinDirection, transform.position.y), 1.5f * Time.deltaTime);
            if (_spinTimer < 0)
            {
                _isReturning = true;
                _isStopped = false;
            }

            _hitTimer -= Time.deltaTime;
            if (_hitTimer < 0)
            {
                _hitTimer = _hitCooldown;
                var colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                foreach (var hit in colliders)
                {
                    var enemy = hit.GetComponent<Enemy>();
                    if(enemy != null)
                        SkillDamage(enemy);
                }
            }
        }
    }

    private void SkillDamage(Enemy enemy)
    {
        PlayerManager.Instance.Player.Stats.DoDamage(enemy);
        if(SkillManager.Instance.ThrowSword.TimeStopUnlocked)
            enemy.FreezeTime(_freezeTimeDuration);
        if(SkillManager.Instance.ThrowSword.VolnerableUnlocked)
            enemy.Stats.OnVulnerable(_freezeTimeDuration);

        var equipedAmulet = InventoryManager.Instance.GetEquiped(EquipmentType.Amulet);
        if (equipedAmulet != null)
            equipedAmulet.OnEffect(enemy.transform);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(_isReturning)
            return;

        var enemy = col.GetComponent<Enemy>();
        if (enemy != null)
        {
            SkillDamage(enemy);
            SetupTargetsForBounce();
        }

        StuckInto(col);
    }

    private void SetupTargetsForBounce()
    {
        if (_swordType == SwordType.Bounce && _enemyTarget.Count <= 0)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 3);
            foreach (var hit in colliders)
            {
                if (hit.GetComponent<Enemy>() != null)
                    _enemyTarget.Add(hit.transform);
            }
        }
    }

    private void StuckInto(Collider2D collider2D)
    {
        if (_swordType == SwordType.Pierce)
        {
            if(_amount > 0 && collider2D.GetComponent<Enemy>() != null)
            {
                _amount--;
                return;
            }
        }

        if (_swordType == SwordType.Spin)
        {
            StopWhenSpinning();
            return;
        }
        
        _canRotate = false;
        _collider.enabled = false;

        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _fx.Play();

        if (_swordType == SwordType.Bounce && _enemyTarget.Count > 0)
            return;

        _animator.SetBool("Rotation", false);
        transform.parent = collider2D.transform;
    }
}
