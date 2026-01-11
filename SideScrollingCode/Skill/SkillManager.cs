using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public DashSkill Dash { get; private set; }
    public CloneSkill Clone { get; private set; }
    public ThrowSwordSkill ThrowSword { get; private set; }
    public BlackholeSkill BlackholeSkill { get; private set; }
    public CrystalSkill CrystalSkill { get; private set; }
    public ParrySkill ParrySkill { get; private set; }
    public DodgeSkill DodgeSkill { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;

        Dash = gameObject.GetComponent<DashSkill>();
        Clone = gameObject.GetComponent<CloneSkill>();
        ThrowSword = gameObject.GetComponent<ThrowSwordSkill>();
        BlackholeSkill = gameObject.GetComponent<BlackholeSkill>();
        CrystalSkill = gameObject.GetComponent<CrystalSkill>();
        ParrySkill = gameObject.GetComponent<ParrySkill>();
        DodgeSkill = gameObject.GetComponent<DodgeSkill>();
    }
}
