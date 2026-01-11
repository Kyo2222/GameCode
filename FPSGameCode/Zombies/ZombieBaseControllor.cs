using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle,
    Chase,
    Attack,
    Dead
}
public class ZombieBaseControllor : MonoBehaviour, IDamagable
{
    protected NavMeshAgent navAgent;
    protected Animator animator;
    [SerializeField]
    protected ZombieState currentState = ZombieState.Idle;
    protected Transform player;
    public float currentHealth { get; set; } = 100;
    [SerializeField]
    protected float chaseDistance = 10f;
    [SerializeField]
    protected float attackDistance = 2f;
    [SerializeField]
    protected float attackCooldown = 2f;
    [SerializeField]
    protected float attackDelay = 1.5f;
    [SerializeField]
    protected int damage = 10;

    protected CapsuleCollider capsuleCollider;
    protected bool isAttacking;
    protected float lastAttackTime;

    [SerializeField]
    protected GameObject bloodScreenEffect;
    private GameObject instantiatedObject;

    protected virtual void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;
        animator = GetComponent<Animator>();

        GetPlayer();
    }

    private void GetPlayer()
    {
        var playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.Log("Player object not found!");
    }

    protected bool IsPlayerInRange(float distance)
    {
        return Vector3.Distance(transform.position, player.position) <= distance;
    }

    protected IEnumerator ActivateBloodScreenEffect()
    {
        InstantiateObject();
        yield return new WaitForSeconds(attackDelay);
        DeleteObject();
    }

    public void TakeDamage(int damageAmount)
    {
        if(currentState == ZombieState.Dead)
            return;
        
        currentHealth -= damageAmount;
        if (!(currentHealth <= 0)) 
            return;
        
        currentHealth = 0;
        Die();
    }

    private void Die()
    {
        currentState = ZombieState.Dead;
        player = null;
    }

    private void InstantiateObject()
    {
        instantiatedObject = Instantiate(bloodScreenEffect);
    }

    private void DeleteObject()
    {
        if (instantiatedObject == null) 
            return;
        
        Destroy(instantiatedObject);

        instantiatedObject = null;
    }

}
