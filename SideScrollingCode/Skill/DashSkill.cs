using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("Dash")]
    [SerializeField]
    private UISkillSlot _dashButton;
    public bool DashUnlocked { get; private set; }

    [Header("Clone on dash")]
    [SerializeField]
    private UISkillSlot _cloneOnDashButton;

    private bool _cloneOnDashUnlocked;

    [Header("Clone on arrival")]
    [SerializeField]
    private UISkillSlot _cloneOnArrivalButton;

    private bool _cloneOnArrivalUnlocked;

    private void Awake()
    {
        _cooldown = 1;
    }

    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        OnlockCloneOnArrival();
    }

    public override void AddListener()
    {
        _dashButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        _cloneOnDashButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        _cloneOnArrivalButton.GetComponent<Button>().onClick.AddListener(OnlockCloneOnArrival);
    }
    
    private void UnlockDash()
    {
        if(_dashButton.Unlocked)
            DashUnlocked = true;
    }

    private void UnlockCloneOnDash()
    {
        if(_cloneOnDashButton.Unlocked)
            _cloneOnDashUnlocked = true;
    }

    private void OnlockCloneOnArrival()
    {
        if(_cloneOnArrivalButton.Unlocked)
            _cloneOnArrivalUnlocked = true;
    }

    public void CreateDashClone(bool isStart)
    {
        if (isStart)
        {
            if (_cloneOnDashUnlocked)
                SkillManager.Instance.Clone.CreateClone(_player.transform, Vector3.zero);
        }
        else
        {
            if (_cloneOnArrivalUnlocked)
                SkillManager.Instance.Clone.CreateClone(_player.transform, Vector3.zero);
        }
    }
}
