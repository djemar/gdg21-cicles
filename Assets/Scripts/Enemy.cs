using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public NavMeshAgent agent;

    public Transform player, attackPoint;

    public LayerMask whatIsGround, whatIsPlayer;

    private bool isDead = false;

    //Patroling
    private float startingX;
    private float startingY;
    private float startingZ;
    private Vector3 dest;
    private Vector3 ground;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    public float deathTimer;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
   

    private void Awake()
    {
        startingX = transform.position.x;
        startingY = transform.position.y;
        startingZ = transform.position.z;
    }

    private void Patroling()
    {
        dest = new Vector3(startingX, startingY, startingZ);
        agent.SetDestination(dest);
    }

    private void ChasePlayer()
    {
        Vector3 pos = new Vector3(player.position.x, 1f, player.position.z);
        agent.SetDestination(pos);
        //agent.SetDestination(player.position);
    }


    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            // Animate attack
            // Deal damage
            alreadyAttacked = true;
            animator.SetTrigger("AttackPlayer");
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("isDead");
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            Destroy(gameObject, deathTimer);
            Debug.Log("Enemy is dead.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !isDead)
            Patroling();

        if (playerInSightRange && !playerInAttackRange && !isDead)
            ChasePlayer();

        if (playerInSightRange && playerInAttackRange && !isDead)
            AttackPlayer();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void EnemyMeleeAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsPlayer);

        foreach (var p in hitPlayer)
        {
            p.GetComponent<PlayerCombat>().TakeDamage();
        }
    }

    private void Squish()
    {
        FindObjectOfType<AudioManager>().Play("Squish");
    }
}
