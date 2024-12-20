using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public float health;

    public LayerMask whatIsGround, whatIsPlayer;

    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    // Patrolling 
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    //public GameObject projectile;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Awake()
    {
        player = GameObject.Find("ThirdPersonController_LITE").transform; // find player object
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if (!playerInSightRange && !playerInAttackRange) { Patrol(); }
        if (playerInSightRange && !playerInAttackRange) { ChasePlayer(); }
        if (playerInSightRange && playerInAttackRange) { AttackPlayer(); }

    }

    private void Patrol()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) // Walkpoint reached
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {

        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
           

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Health healthScript = GetComponent<Health>();

        if (healthScript != null)
        {
            healthScript.health = currentHealth;
        }

        healthSlider.value = currentHealth;
        /*Invoke(nameof(DestroyEnemy), .5f)*/
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy died.");
            gameObject.SetActive(false);
            healthSlider.gameObject.SetActive(false);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
