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

    //Patroling
    private float startingX;
    private float startingY;
    private float startingZ;
    public Vector3 dest;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Amy").transform;
        agent = GetComponent<NavMeshAgent>();
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

    // Start is called before the first frame update
    void Start()
    {

    }

    public void TakeDamage()
    {
        Die();
    }

    void Die()
    {
        Debug.Log("Enemy is dead.");
        //animator.SetBool("isDead", true);
        GetComponent<SphereCollider>().enabled = false;
        this.enabled = false;
        //Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();

        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();

        if (playerInSightRange && playerInAttackRange)
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
        Debug.Log("Event on enemy animation");
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsPlayer);

        foreach(var player in hitPlayer) 
            player.GetComponent<PlayerCombat>().TakeDamage();
    }
}
