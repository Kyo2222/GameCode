using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpellController : MonoBehaviour
{
    [SerializeField]
    private Transform _checkPosition;
    [SerializeField]
    private Vector2 _boxSize;

    private float _defaultSize = 1.5f;

    private int _playerMask;
    private CharacterStats _stats;

    private void Awake()
    {
        _playerMask = LayerMask.GetMask("Player");
    }

    public void SetupSpell(CharacterStats stats)
    {
        _stats = stats;
        transform.localScale = Vector3.one * _defaultSize;
    }

    private void SpellTirgger()
    {
        var colliders = Physics2D.OverlapBoxAll(_checkPosition.position, _boxSize, _playerMask);
        foreach (var hit in colliders)
        {
            var player = hit.GetComponent<Player>();
            if (player != null)
            {
                player.SetupKnockbackDir(transform);
                _stats.DoDamage(hit.GetComponent<Character>());
            }
        }
    }

    private void RealseSelf()
    {
        ObjPoolManager.Instance.SpellCastPool.Release(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_checkPosition.position, _boxSize);
    }
}
