using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [SerializeField]
    private float _cloneDuration;
    [SerializeField]
    private float _attackMultiplier;

    [Header("Clone attack")]
    [SerializeField]
    private UISkillSlot _cloneAttackButton;
    [SerializeField]
    private float _cloneAttackMultiplier;
    private bool _canAttack;

    [Header("Aggresive clone")]
    [SerializeField]
    private UISkillSlot _aggresiveCloneButton;
    [SerializeField]
    private float _aggresiveCloneAttackMultiplier;
    public bool CanApplyOnHitEffect { get; private set; }

    [Header("Multiple clone")]
    [SerializeField]
    private UISkillSlot _multipleCloneButton;
    [SerializeField]
    private float _multipleCloneAttackMultiplier;
    [SerializeField]
    private bool _canDuplicateClone;
    [SerializeField]
    private float _chanceToDuplicate;
    [Header("Crystal instead of clone")]
    [SerializeField]
    private UISkillSlot _crystalInseadButton;
    [SerializeField]
    private bool _crystalInseadClone;
    public bool CrystalInseadClone
    {
        get { return _crystalInseadClone; }
    }


    #region Unlock

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveCloneAttack();
        UnlockCrystalCloneAttack();
        UnlockMultipleCloneAttack();
    }

    public override void AddListener()
    {
        _cloneAttackButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        _aggresiveCloneButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveCloneAttack);
        _multipleCloneButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleCloneAttack);
        _crystalInseadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalCloneAttack);
    }

    private void UnlockCloneAttack()
    {
        if (_cloneAttackButton.Unlocked)
        {
            _attackMultiplier = _cloneAttackMultiplier;
            _canAttack = true;
        }
    }

    private void UnlockAggresiveCloneAttack()
    {
        if (_aggresiveCloneButton.Unlocked)
        {
            _attackMultiplier = _aggresiveCloneAttackMultiplier;
            CanApplyOnHitEffect = true;
        }
    }

    private void UnlockMultipleCloneAttack()
    {
        if (_multipleCloneButton.Unlocked)
        {
            _attackMultiplier = _multipleCloneAttackMultiplier;
            _canDuplicateClone = true;
        }
    }

    private void UnlockCrystalCloneAttack()
    {
        if (_crystalInseadButton.Unlocked)
            _crystalInseadClone = true;
    }

    #endregion
    
    public void CreateClone(Transform transform, Vector3 offset)
    {
        if (_crystalInseadClone)
        {
            SkillManager.Instance.CrystalSkill.CreateCrystal();
            return;
        }
        
        if(!_canAttack)
            return;
        
        var newClone = ObjPoolManager.Instance.SkillPoolDictionary[(int) SkillPoolType.Clone].Get();
        newClone.GetComponent<CloneSkillController>().SetupClone(transform, _canAttack, _cloneDuration,
            _canDuplicateClone, _chanceToDuplicate, offset, FindClosestEnemy(newClone.transform), _attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform enemy)
    {
        StartCoroutine(CreateCloneWithDelay(enemy, new Vector3(1 * _player.FlipDir, 0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform transform, Vector3 offset)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(transform, offset);
    }
}
