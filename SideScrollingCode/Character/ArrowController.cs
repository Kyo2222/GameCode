using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArrowController : MonoBehaviour
{
    [FormerlySerializedAs("_damege")]
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float xVelocity;
    private Rigidbody2D _rb;
    private ParticleSystem _particle;
    private CapsuleCollider2D _collider;

    private CharacterStats _stats;
    private string _targetLayerName = "Player";
    private bool _canMove = true;
    private bool _flipped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _particle = GetComponentInChildren<ParticleSystem>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if(_canMove)
            _rb.velocity = new Vector2(xVelocity, _rb.velocity.y);
    }

    public void OnInit(float speed, float flipDir, CharacterStats stats)
    {
        _targetLayerName = "Player";
        _stats = stats;
        transform.rotation = Quaternion.identity;
        _collider.enabled = true;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        xVelocity = speed * flipDir;
        transform.localScale = new Vector3(transform.localScale.x * flipDir, transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(_targetLayerName))
        {
            _stats.DoDamage(col.GetComponent<Character>());
            StuckInto(col);
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            StuckInto(col);
    }

    private void StuckInto(Collider2D col)
    {
        _particle.Stop();
        _collider.enabled = false;
        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = col.transform;
    }

    public void FlipArrow()
    {
        if(_flipped)
            return;

        xVelocity = xVelocity * -1;
        _flipped = true;
        transform.Rotate(0, 180, 0);
        _targetLayerName = "Enemy";
    }
}
