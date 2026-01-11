using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : ZombieBaseControllor
{
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (currentState)
        {
            case ZombieState.Idle:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", false);
                if(IsPlayerInRange(chaseDistance))
                    currentState = ZombieState.Chase;
                break;
            case ZombieState.Chase:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsAttacking", false);
                navAgent.SetDestination(player.position);
                if(IsPlayerInRange(attackDistance))
                    currentState = ZombieState.Attack;
                break;
            case ZombieState.Attack:
                animator.SetBool("IsAttacking", true);
                navAgent.SetDestination(transform.position);
                if(!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(AttackWithDelay());
                    Debug.Log("Attack Player");
                    StartCoroutine(ActivateBloodScreenEffect());
                }
                if(Vector3.Distance(transform.position, player.position) > attackDistance)
                    currentState = ZombieState.Chase;
                break;
            case ZombieState.Dead:
                animator.SetBool("IsWalking", false);
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

    private IEnumerator AttackWithDelay()
    {
        isAttacking = true;

        //damage the player
        var playerMovement = player.GetComponent<IDamagable>();
        if(playerMovement != null)
            playerMovement.TakeDamage(damage);

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        lastAttackTime = Time.time;
    }
}
