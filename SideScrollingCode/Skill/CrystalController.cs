using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrystalController : MonoBehaviour
{
    private Animator _anim;
    private float _duration;
    private bool _canExplosive;
    private bool _canMove;
    private float _moveSpeed;

    private bool _canGrow;
    private float _growSpeed = 5f;
    private Transform _target;

    public void SetupCrystal(float duration, bool canExplosive, bool canMove, float moveSpeed, Transform closestEnemy)
    {
        transform.localScale = Vector3.one;
        _anim = GetComponent<Animator>(); 
        _duration = duration;
        _canExplosive = canExplosive;
        _canMove = canMove;
        _canGrow = false;
        _moveSpeed = moveSpeed;
        _target = closestEnemy;
    }

    public void ChooseRandomEnemy()
    {
        var enemyMask = LayerMask.GetMask("Enemy");
        var radius = 8;
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        if(colliders.Length > 0)
            _target = colliders[Random.Range(0, colliders.Length)].transform;
    }

    private void Update()
    {
        _duration -= Time.deltaTime;
        if(_duration < 0)
        {
            FinishedCrystal();
        }

        if (_canMove)
        {
            if(_target == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position, _target.position, _moveSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, _target.position) < 1)
            {
                FinishedCrystal();
                _canMove = false;
            }
        }

        if (_canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), _growSpeed * Time.deltaTime);
    }

    public void FinishedCrystal()
    {
        if (_canExplosive)
        {
            _anim.SetTrigger("Explode");
            var animCallback = _anim.GetComponent<AnimEvents>();
            animCallback.HitCallBack((() =>
            {
                _canGrow = true;
                var radius = GetComponent<CircleCollider2D>().radius;
                var colliders = Physics2D.OverlapCircleAll(transform.position, radius);
                foreach (var hit in colliders)
                {
                    var enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.SetupKnockbackDir(transform);
                        PlayerManager.Instance.Player.Stats.DoMagicalDamage(enemy);
                        var equipedAmulet = InventoryManager.Instance.GetEquiped(EquipmentType.Amulet);
                        if(equipedAmulet != null)
                            equipedAmulet.OnEffect(enemy.transform);
                    }
                }
            }));
            
            animCallback.EndAnimCallBack((() =>
            {
                ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.Crystal, gameObject);
                SkillManager.Instance.CrystalSkill.ReleaseCrystal();
            }));
        }
        else
        {
            ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.Crystal, gameObject);
            SkillManager.Instance.CrystalSkill.ReleaseCrystal();
        }
    }
}
