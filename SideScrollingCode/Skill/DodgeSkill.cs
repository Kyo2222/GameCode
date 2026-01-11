using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("Dodge")]
    [SerializeField]
    private UISkillSlot _dodgeButton;
    [SerializeField]
    private int _evasionAmount;
    private bool _dodgeUnlocked;

    [Header("Mirage Dodge")]
    [SerializeField]
    private UISkillSlot _mirageButton;
    private bool _mirageUnlocked;

    public override void AddListener()
    {
        _dodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        _mirageButton.GetComponent<Button>().onClick.AddListener(UnlockMirage);
    }

    private void UnlockDodge()
    {
        if (_dodgeButton.Unlocked && !_dodgeUnlocked)
        {
            if(_dodgeUnlocked)
                return;
            
            _player.Stats.Evasion.AddModifier(_evasionAmount);
            InventoryManager.Instance.UpdateAbility();
            _dodgeUnlocked = true;
        }
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirage();
    }

    private void UnlockMirage()
    {
        if (_mirageButton.Unlocked)
            _mirageUnlocked = true;
    }

    public void CreateMirageDodge()
    {
        if (_mirageUnlocked)
            SkillManager.Instance.Clone.CreateClone(_player.transform, new Vector3(2 * _player.FlipDir, 0));
    }
}
