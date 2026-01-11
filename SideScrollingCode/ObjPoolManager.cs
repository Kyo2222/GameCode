using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillPoolType
{
    Clone = 0,
    HotKey = 1,
    Crystal = 2
}

public enum ElementsType
{
    None = 0,
    Fire = 1,
    Ice = 2,
    Lighting = 3,
}

public enum ItemType
{
    Matrial = 1,
    Equipment = 2,
}

public enum EquipmentEffectType
{
    ThunderStrike = 1,
    IceAndFire = 2,
    Heal = 3,
    Buff = 4,
    FreezeEnemies = 5,
}

public enum FXType
{
    HitFx,
    HitcriticalFx,
    PopUpText,
    AfterImage,
}
public class ObjPoolManager : MonoBehaviour
{
    public static ObjPoolManager Instance;
    private Dictionary<int, GameObjectPool> _skillPoolDictionary = new Dictionary<int, GameObjectPool>();
    public Dictionary<int, GameObjectPool> SkillPoolDictionary
    {
        get { return _skillPoolDictionary; }
    }

    private Dictionary<int, GameObjectPool> _elementPoolDictionary = new Dictionary<int, GameObjectPool>();
    public Dictionary<int, GameObjectPool> ElementPoolDictionary
    {
        get { return _elementPoolDictionary; }
    }

    private Dictionary<int, GameObjectPool> _dropItemPoolDictionary = new Dictionary<int, GameObjectPool>();

    public Dictionary<int, GameObjectPool> DropItemPoolDictionary
    {
        get { return _dropItemPoolDictionary; }
    }

    private Dictionary<int, GameObjectPool> _effectPoolDictionary = new Dictionary<int, GameObjectPool>();

    public Dictionary<int, GameObjectPool> EffectPoolDictionary
    {
        get { return _effectPoolDictionary; }
    }

    public GameObjectPool ArrowPool { get; private set; }
    public GameObjectPool ExplosionPool { get; private set; }
    protected Dictionary<int, GameObjectPool> _fxObjPoolDictionary = new Dictionary<int, GameObjectPool>();
    public GameObjectPool SpellCastPool { get; private set; }

    [Header("Skill")]
    [SerializeField]
    private GameObject _clonePrefab;
    [SerializeField]
    private GameObject _hotKeyPrefab;
    [SerializeField]
    private GameObject _crystalPrefab;

    [Header("Elements")]
    [SerializeField]
    private GameObject _thunderPrefab;

    [Header("Item")]
    [SerializeField]
    private GameObject _dropItemPrefab;

    [Header("Effect")]
    [SerializeField]
    private GameObject _thunderStrikePrefab;
    [SerializeField]
    private GameObject _iceAndFirePrefab;

    [Space]
    [Header("FX")]
    [SerializeField]
    private GameObject _hitFx;
    [SerializeField]
    private GameObject _hitcriticalFx;
    [SerializeField]
    private GameObject _afterImagePrefab;
    [Header("Pop up text")]
    [SerializeField]
    private GameObject _popUpTextPrefab;
    
    [Space]
    [Header("Arrow")]
    [SerializeField]
    private GameObject _arrowPrefab;
    [Header("Explosion")]
    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("spellCast FX")]
    [SerializeField]
    private GameObject _spellCastPrefab;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
        
        _skillPoolDictionary.Add((int) SkillPoolType.Clone, new GameObjectPool(_clonePrefab, gameObject));
        _skillPoolDictionary.Add((int) SkillPoolType.HotKey, new GameObjectPool(_hotKeyPrefab, gameObject));
        _skillPoolDictionary.Add((int) SkillPoolType.Crystal, new GameObjectPool(_crystalPrefab, gameObject));
        _elementPoolDictionary.Add((int) ElementsType.Lighting, new GameObjectPool(_thunderPrefab, gameObject));
        _dropItemPoolDictionary.Add((int) ItemType.Matrial, new GameObjectPool(_dropItemPrefab, gameObject));
        _effectPoolDictionary.Add((int) EquipmentEffectType.ThunderStrike, new GameObjectPool(_thunderStrikePrefab, gameObject));
        _effectPoolDictionary.Add((int) EquipmentEffectType.IceAndFire, new GameObjectPool(_iceAndFirePrefab, gameObject));
        ArrowPool = new GameObjectPool(_arrowPrefab, gameObject);
        ExplosionPool = new GameObjectPool(_explosionPrefab, gameObject);
        _fxObjPoolDictionary.Add((int) FXType.HitFx, new GameObjectPool(_hitFx, gameObject));
        _fxObjPoolDictionary.Add((int) FXType.HitcriticalFx, new GameObjectPool(_hitcriticalFx, gameObject));
        _fxObjPoolDictionary.Add((int) FXType.PopUpText, new GameObjectPool(_popUpTextPrefab, gameObject));
        _fxObjPoolDictionary.Add((int) FXType.AfterImage, new GameObjectPool(_afterImagePrefab, gameObject));
        SpellCastPool = new GameObjectPool(_spellCastPrefab, gameObject);
    }

    public GameObject GetClone<T>(int key, Vector3 position)
    {
        if (typeof(T) == typeof(EquipmentEffectType))
            return _effectPoolDictionary[key].Get(position, Quaternion.identity);
        else if (typeof(T) == typeof(ItemType))
            return _dropItemPoolDictionary[key].Get(position, Quaternion.identity);
        else if (typeof(T) == typeof(ElementsType))
            return _elementPoolDictionary[key].Get(position, Quaternion.identity);
        else if (typeof(T) == typeof(SkillPoolType))
            return _skillPoolDictionary[key].Get(position, Quaternion.identity);
        else if (typeof(T) == typeof(FXType))
            return _fxObjPoolDictionary[key].Get(position, Quaternion.identity);
        else
        {
            Debug.Log("not obj");
            return null;
        }
    }
    
    public void ReleaseClone<T>(int key, GameObject obj)
    {
        if(typeof(T) == typeof(EquipmentEffectType))
            _effectPoolDictionary[key].Release(obj);
        else if(typeof(T) == typeof(ItemType))
            _dropItemPoolDictionary[key].Release(obj);
        else if (typeof(T) == typeof(ElementsType))
            _elementPoolDictionary[key].Release(obj);
        else if (typeof(T) == typeof(SkillPoolType))
            _skillPoolDictionary[key].Release(obj);
        else if (typeof(T) == typeof(FXType))
            _fxObjPoolDictionary[key].Release(obj);
        else
        {
            Debug.Log("not obj");
        }
    }

    private void OnDestroy()
    {
        foreach (var data in _skillPoolDictionary)
        {
            data.Value.Clear();
        }

        foreach (var data in _elementPoolDictionary)
        {
            data.Value.Clear();
        }

        foreach (var data in _dropItemPoolDictionary)
        {
            data.Value.Clear();
        }

        foreach (var data in _effectPoolDictionary)
        {
            data.Value.Clear();
        }

        foreach (var data in _fxObjPoolDictionary)
        {
            data.Value.Clear();
        }

        ArrowPool.Clear();
    }
}
