using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum SwordType
{
    Normal,
    Bounce,
    Pierce,
    Spin
}

public class ThrowSwordSkill : Skill
{
    [SerializeField]
    private SwordType _swordType = SwordType.Normal;
    
    [SerializeField]
    private GameObject _swordPrefab;
    [SerializeField]
    private Vector2 _launchDir;

    private float _swordGravity;

    [SerializeField]
    private float _freezeTimeDuration = 0.7f;
    [SerializeField]
    private float _returnSpeed = 1;
    
    [Header("Normal Info")]
    [SerializeField]
    private UISkillSlot _swordButton;
    public bool SwordUnlocked { get; private set; }
    [SerializeField]
    private Vector2 _normalLaunchDir;
    [SerializeField]
    private float _normalGravity;
    
    [Header("Bounce Info")]
    [SerializeField]
    private UISkillSlot _bounceButton;
    [SerializeField]
    private float _bounceGravity;
    [SerializeField]
    private int _bounceAmount = 4;
    [SerializeField]
    private float _bounceSpeed = 20;
    [SerializeField]
    public Vector2 _bounceLaunchDir;
    
    [Header("Peirce Info")]
    [SerializeField]
    private UISkillSlot _peirceButton;
    [SerializeField]
    private float _pierceGravity;
    [SerializeField]
    private int _pierceAmount;
    [SerializeField]
    public Vector2 _pierceLaunchDir;
    
    [Header("Spin Info")]
    [SerializeField]
    private UISkillSlot _spinButton;
    [SerializeField]
    private float _spinGravity = 1;
    [SerializeField]
    private float _maxTravelDistance = 7;
    [SerializeField]
    private float _spinDuration = 2;
    [SerializeField]
    private float _hitCooldown = 0.35f;
    [SerializeField]
    public Vector2 _spinLaunchDir;
    
    [Header("Passive skills")]
    [SerializeField]
    private UISkillSlot _timeStopButton;
    public bool TimeStopUnlocked { get; private set; }
    [SerializeField]
    private UISkillSlot _volnerableButton;
    public bool VolnerableUnlocked { get; private set; }

    #region Unlock

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockTimeStop();
        UnlockVulnerable();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
    }

    public override void AddListener()
    {
        _swordButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        _timeStopButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        _volnerableButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
        _bounceButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        _peirceButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        _spinButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
    }
    private void UnlockTimeStop()
    {
        if (_timeStopButton.Unlocked)
            TimeStopUnlocked = true;
    }

    private void UnlockVulnerable()
    {
        if (_volnerableButton.Unlocked)
            VolnerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if (_swordButton.Unlocked)
        {
            _swordType = SwordType.Normal;
            SwordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if (_bounceButton.Unlocked)
            _swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if (_peirceButton.Unlocked)
            _swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (_spinButton.Unlocked)
            _swordType = SwordType.Spin;
    }

    #endregion

    public void CreateSword()
    {
        GameObject newSword = Instantiate(_swordPrefab, _player.transform.position, _player.transform.rotation);
        var sword = newSword.GetComponent<ThrowSwordSkillController>();
        _player.SetNewSword(newSword);

        SwitchSwordType(sword);

        var launchDir = new Vector2(_launchDir.x * _player.FlipDir, _launchDir.y);
        sword.SetupSword(launchDir, _swordGravity, _freezeTimeDuration, _returnSpeed);
    }

    private void SwitchSwordType(ThrowSwordSkillController sword)
    {
        switch (_swordType)
        {
            case SwordType.Normal:
                _swordGravity = _normalGravity;
                _launchDir = _normalLaunchDir;
                break;
            case SwordType.Bounce:
                _swordGravity = _bounceGravity;
                _launchDir = _bounceLaunchDir;
                sword.SetupBounce(_swordType, _bounceAmount, _bounceSpeed);
                break;
            case SwordType.Pierce:
                _swordGravity = _pierceGravity;
                _launchDir = _pierceLaunchDir;
                sword.SetupPierce(_swordType, _pierceAmount);
                break;
            case SwordType.Spin:
                _swordGravity = _spinGravity;
                _launchDir = _spinLaunchDir;
                sword.SetupSpin(_swordType, _maxTravelDistance, _spinDuration, _hitCooldown);
                break;
        }
    }
}
