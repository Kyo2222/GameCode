using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    private CrystalController _currentCrystal;
    [SerializeField]
    private float _duration = 1.5f;

    [Header("Mirage")]
    [SerializeField]
    private UISkillSlot _cloneInstaedButton;
    private bool _cloneInsteadCrystal;

    [Header("Simple")]
    [SerializeField]
    private UISkillSlot _crystalButton;
    public bool CrystalUnlocked { get; private set; }

    [Header("Explosive")]
    [SerializeField]
    private UISkillSlot _explosiveButton;
    [SerializeField]
    private float _explisoveCooldown;
    private bool _canExplosive;

    [Header("Moving")]
    [SerializeField]
    private UISkillSlot _movingCrystalButton;
    private bool _canMoveToEnemy;
    [SerializeField]
    private float _moveSpeed;

    [Header("Multi stacking")]
    [SerializeField]
    private UISkillSlot _multiStackButton;
    private bool _canMultiStack;
    [SerializeField]
    private int _stackAmount;
    [SerializeField]
    private float _multiStackCooldown;
    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultiCrystal())
            return;

        if (_currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(_canMoveToEnemy)
                return;

            // player move to crystal
            var playerPos = _player.transform.position;
            _player.transform.position = _currentCrystal.transform.position;
            _currentCrystal.transform.position = playerPos;

            if (_cloneInsteadCrystal)
            {
                var skillManager = SkillManager.Instance;
                skillManager.Clone.CreateClone(_currentCrystal.transform, Vector3.zero);
                ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.Crystal, _currentCrystal.gameObject);
                _currentCrystal = null;
            }
            else
            {
                _currentCrystal.FinishedCrystal();
            }
        }
    }

    #region Unlock skill

    protected override void CheckUnlock()
    {
        UnlockCloneInstaed();
        UnlockCrystal();
        UnlockExplosive();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }
    public override void AddListener()
    {
        _cloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCloneInstaed);
        _crystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        _explosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosive);
        _movingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        _multiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);
    }

    private void UnlockCloneInstaed()
    {
        if (_cloneInstaedButton.Unlocked)
            _cloneInsteadCrystal = true;
    }

    private void UnlockCrystal()
    {
        if (_crystalButton.Unlocked)
            CrystalUnlocked = true;
    }

    private void UnlockExplosive()
    {
        if (_explosiveButton.Unlocked)
        {
            _canExplosive = true;
            _cooldown = _explisoveCooldown;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (_movingCrystalButton.Unlocked)
            _canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if (_multiStackButton.Unlocked)
            _canMultiStack = true;
    }

    #endregion

    public void CreateCrystal()
    {
        _currentCrystal = ObjPoolManager.Instance.GetClone<SkillPoolType>((int) SkillPoolType.Crystal,
                _player.transform.position).GetComponent<CrystalController>();
        _currentCrystal.SetupCrystal(_duration, _canExplosive, _canMoveToEnemy, _moveSpeed,
            FindClosestEnemy(_currentCrystal.transform));
    }

    public void CrystalChooseRandomTarget()
    {
        _currentCrystal.ChooseRandomEnemy();
    }

    private bool CanUseMultiCrystal()
    {
        if (_canMultiStack)
        {
            CreateMultiStack();

            _cooldown = _multiStackCooldown;
            return true;
        }

        return false;
    }

    private async void CreateMultiStack()
    {
        for (int i = 0; i < _stackAmount; i++)
        {
            var crystal = ObjPoolManager.Instance.GetClone<SkillPoolType>((int) SkillPoolType.Crystal,
                    _player.transform.position).GetComponent<CrystalController>();

            crystal.SetupCrystal(_duration, _canExplosive, _canMoveToEnemy, _moveSpeed,
                FindClosestEnemy(crystal.transform));
            
            await Task.Delay(150);
        }
    }
    
    public void ReleaseCrystal()
    {
        _currentCrystal = null;
    }
}
