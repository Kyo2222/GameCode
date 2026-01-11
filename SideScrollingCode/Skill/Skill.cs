using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Skill : MonoBehaviour
{
    [SerializeField]
    protected float _cooldown;
    public float Cooldown
    {
        get { return _cooldown; }
    }
    public float CooldownTimer { get; private set; }
    
    protected Player _player;

    protected virtual void Start()
    {
        _player = PlayerManager.Instance.Player;
        CheckUnlock();
    }

    protected void Update()
    {
        CooldownTimer -= Time.deltaTime;
    }

    public virtual void AddListener()
    {
        
    }

    protected virtual void CheckUnlock()
    {
        
    }

    public virtual bool CanUseSkill()
    {
        if (CooldownTimer < 0)
        {
            UseSkill();
            CooldownTimer = _cooldown;
            return true;
        }

        _player.Stats.Fx.CreatePopUpText("Cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
        
    }

    protected virtual Transform FindClosestEnemy(Transform checkTransform)
    {
        var colliders = Physics2D.OverlapCircleAll(checkTransform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                var distanceToEnemy = Vector2.Distance(checkTransform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return closestEnemy;
    }
}
