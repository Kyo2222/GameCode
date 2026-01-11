using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackholeSkill : Skill
{
    [SerializeField]
    private UISkillSlot _blackholeButton;
    public bool BlackholeUnlocked { get; private set; }
    [SerializeField]
    private GameObject _blockHolePrefab;
    [SerializeField]
    private float _duration;
    [SerializeField]
    private int _attackAmount;
    [SerializeField]
    private float _cloneCooldown;
    [SerializeField]
    private float _maxSize;
    [SerializeField]
    private float _growSpeed;
    [SerializeField]
    private float _shrinkSpeed;

    private BlackholeSkillController _currentBlackhole;

    public override void AddListener()
    {
        _blackholeButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }

    private void UnlockBlackhole()
    {
        if (_blackholeButton.Unlocked)
            BlackholeUnlocked = true;
    }
    
    public override void UseSkill()
    {
        base.UseSkill();
        _currentBlackhole = Instantiate(_blockHolePrefab, _player.transform.position, Quaternion.identity)
            .GetComponent<BlackholeSkillController>();
        _currentBlackhole.SetupBlackhole(_maxSize, _growSpeed, _shrinkSpeed, _attackAmount, _cloneCooldown, _duration);
        
        AudioManager.Instance.PlaySE(3, _player.transform);
        AudioManager.Instance.PlaySE(6, _player.transform);
    }

    public bool FinishedSkill()
    {
        if (!_currentBlackhole)
            return false;
        
        if (_currentBlackhole.PlayerCanExitState)
        {
            _currentBlackhole = null;
            return true;
        }
        
        return false;
    }
}
