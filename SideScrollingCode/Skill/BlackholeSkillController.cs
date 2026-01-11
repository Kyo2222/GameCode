using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    private List<KeyCode> _keyCodeList = new List<KeyCode>(){KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.F, KeyCode.T};

    private float _maxSize;
    private float _growSpeed;
    private float _shrinkSpeed;
    private float _timer;
    
    private bool _canGrow = true;
    private bool _canShrink;
    private bool _canCreateHotKeys = true;
    private bool _playerCanDisapear = true;
    private List<Transform> _targetList = new List<Transform>();
    private List<GameObject> _createHotKeyList = new List<GameObject>();
    
    private bool _cloneAttackReleased;
    private int _attackAmount = 4;
    private float _cloneAttackCooldown = 0.3f;
    private float _cloneAttackTimer;
    
    public bool PlayerCanExitState { get; private set; }

    public void SetupBlackhole(float maxSize, float growSpeed, float shrinkSpeed, int attackAmount, float cloneAttackCooldown, float duration)
    {
        _maxSize = maxSize;
        _growSpeed = growSpeed;
        _shrinkSpeed = shrinkSpeed;
        _attackAmount = attackAmount;
        _cloneAttackCooldown = cloneAttackCooldown;
        _timer = duration;
        if (SkillManager.Instance.Clone.CrystalInseadClone)
            _playerCanDisapear = false;
    }
    
    private void Update()
    {
        _cloneAttackTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = Mathf.Infinity;
            if (_targetList.Count > 0)
                ReleaseCloneAttack();
            else
                FinishedBlackhole();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            // clone attack
            ReleaseCloneAttack();
        }
        
        CloneAttack();
        
        if (_canGrow && !_canShrink)
            transform.localScale = 
                Vector2.Lerp(transform.localScale, new Vector2(_maxSize, _maxSize), _growSpeed * Time.deltaTime);
        if (_canShrink)
        {
            transform.localScale =
                Vector2.Lerp(transform.localScale, new Vector2(-1, -1), _shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if(_targetList.Count <= 0)
            return;

        DestroyHotKeyList();
        _cloneAttackReleased = true;
        _canCreateHotKeys = false;
        if(_playerCanDisapear)
        {
            _playerCanDisapear = false;
            PlayerManager.Instance.Player.CharacterFX.MakeTransprent(true);
        }
    }

    private void CloneAttack()
    {
        if (_cloneAttackTimer < 0 && _cloneAttackReleased && _attackAmount > 0)
        {
            _cloneAttackTimer = _cloneAttackCooldown;
            var random = Random.Range(0, _targetList.Count);
            int xOffset = Random.Range(0, 100) > 50 ? 1 : -1;

            if (SkillManager.Instance.Clone.CrystalInseadClone)
            {
                SkillManager.Instance.CrystalSkill.CreateCrystal();
                SkillManager.Instance.CrystalSkill.CrystalChooseRandomTarget();
            }
            else
                SkillManager.Instance.Clone.CreateClone(_targetList[random], new Vector3(xOffset, 0));
            
            _attackAmount--;

            if (_attackAmount <= 0)
            {
                Invoke("FinishedBlackhole", 1f);
            }
        }
    }

    private void FinishedBlackhole()
    {
        DestroyHotKeyList();
        PlayerCanExitState = true;
        _cloneAttackReleased = false;
        _canShrink = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.FreezeTime(true);

            CreateHotKey(col);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D col)
    {
        if(_keyCodeList.Count <= 0)
        {
            Debug.Log("Not enough hot keys in list");
            return;
        }

        if (!_canCreateHotKeys)
            return;

        GameObject newKey =
            ObjPoolManager.Instance.GetClone<SkillPoolType>((int) SkillPoolType.HotKey, col.transform.position + new Vector3(0, 2));
        _createHotKeyList.Add(newKey);

        KeyCode chooseKey = _keyCodeList[Random.Range(0, _keyCodeList.Count)];
        _keyCodeList.Remove(chooseKey);
        var newHotKey = newKey.GetComponent<BlackholeHotKeyController>();
        newHotKey.SetupHotKey(chooseKey, col.transform, this);
    }

    private void DestroyHotKeyList()
    {
        if(_createHotKeyList.Count <= 0)
            return;

        for (int i = 0; i < _createHotKeyList.Count; i++)
        {
            ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.HotKey, _createHotKeyList[i]);
        }
        _createHotKeyList.Clear();
    }

    public void AddTargetList(Transform enemy)
    {
        _targetList.Add(enemy);
    }
}
