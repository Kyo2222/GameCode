using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThunderController : MonoBehaviour
{
    private CharacterStats _targetStats;
    [SerializeField]
    private float _speed;
    private int _damage;
    
    private Animator _anim;
    private AnimEvents _animCallback;
    private bool _triggered;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _animCallback = _anim.GetComponent<AnimEvents>();
    }

    public void Setup(int damage, CharacterStats targetStats)
    {
        _damage = damage;
        _targetStats = targetStats;
    }

    // Update is called once per frame
    void Update()
    {
        if(_triggered || _targetStats == null)
            return;

        OnMove();
    }

    private void OnMove()
    {
        transform.position =
            Vector2.MoveTowards(transform.position, _targetStats.transform.position, _speed * Time.deltaTime);
        transform.right = transform.position - _targetStats.transform.position;

        if (Vector2.Distance(transform.position, _targetStats.transform.position) < 0.1f)
        {
            _anim.transform.localPosition = new Vector3(0, 0.5f);
            _anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            _triggered = true;
            _anim.SetTrigger("Hit");
            _animCallback.HitCallBack((() =>
            {
                OnDamage();
            }));
            _animCallback.EndAnimCallBack((() =>
            {
                _triggered = false;
                ObjPoolManager.Instance.ReleaseClone<ElementsType>((int) ElementsType.Lighting, gameObject);
            }));
        }
    }

    private void OnDamage()
    {
        _targetStats.ApplyShock(true);
        _targetStats.TakeDamage(_damage);
    }
}
