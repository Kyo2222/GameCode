using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("Parry")]
    [SerializeField]
    private UISkillSlot _parryButton;
    public bool ParryUnlocked { get; private set; }
    
    [Header("Parry restore")]
    [SerializeField]
    private UISkillSlot _restoreButton;
    private bool _restoreUnlocked;
    [SerializeField, Range(0, 1)]
    private float _restoreHealthPerentage;

    [Header("Parry with mirage")]
    [SerializeField]
    private UISkillSlot _mirageButton;
    private bool _mirageUnlocked;

    public override void UseSkill()
    {
        base.UseSkill();

        if (_restoreUnlocked)
        {
            var restoreAmount = Mathf.RoundToInt(_player.Stats.GetMaxHealthValue() * _restoreHealthPerentage);
            _player.Stats.IncreaseHealth(restoreAmount);
        }
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryResetore();
        OnlockParryMirage();
    }

    public override void AddListener()
    {
        _parryButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        _restoreButton.GetComponent<Button>().onClick.AddListener(UnlockParryResetore);
        _mirageButton.GetComponent<Button>().onClick.AddListener(OnlockParryMirage);
    }

    private void UnlockParry()
    {
        if (_parryButton.Unlocked)
            ParryUnlocked = true;
    }

    private void UnlockParryResetore()
    {
        if (_restoreButton.Unlocked)
            _restoreUnlocked = true;
    }

    private void OnlockParryMirage()
    {
        if (_mirageButton.Unlocked)
            _mirageUnlocked = true;
    }

    public void MakeMirageOnParry(Transform transform)
    {
        if(_mirageUnlocked)
            SkillManager.Instance.Clone.CreateCloneWithDelay(transform);
    }
}
