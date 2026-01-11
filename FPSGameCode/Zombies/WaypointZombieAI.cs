using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointZombieAI : ZombieBaseControllor
{
    private bool isMoving;

    public WaypointZombieAI(float currentHealth)
    {
        this.currentHealth = currentHealth;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (currentState)
        {
            case ZombieState.Idle:
                if(!isMoving || navAgent.remainingDistance < 0.1f)
                {
                    Walk();
                }

                if(IsPlayerInRange(chaseDistance))
                    currentState = ZombieState.Chase;
                break;
            
            case ZombieState.Chase:
                ChasePlayer();
                if(IsPlayerInRange(attackDistance))
                    currentState = ZombieState.Attack;
                break;

            case ZombieState.Attack:
                AttackPlayer();
                if(!IsPlayerInRange(attackDistance))
                    currentState = ZombieState.Chase;
                break;
            
            case ZombieState.Dead:
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsDead", true);
                navAgent.enabled = false;
                capsuleCollider.enabled = false;
                enabled = false;
                GameManager.Instance.AddCurrentScore(1);
                Debug.Log("Dead");
                break;
        }
    }

    private void Walk()
    {
        navAgent.speed = 0.3f;
        Vector3 randomPosition = RandomNavMeshPosition();
        navAgent.SetDestination(randomPosition);
        isMoving = true;
    }

    private Vector3 RandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);
        return hit.position;
    }

    private void ChasePlayer()
    {
        navAgent.speed = 3f;
        //animations
        animator.SetBool("IsChasing", true);
        animator.SetBool("IsAttacking", false);
        navAgent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //animations
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", true);
        navAgent.SetDestination(transform.position);
        if(!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackWithDelay());
            StartCoroutine(ActivateBloodScreenEffect());
            lastAttackTime = Time.time;

            //damage the player
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if(playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
            }
        }

    }

    private IEnumerator AttackWithDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }
}
