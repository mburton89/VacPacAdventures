using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// SpikeEnemy that wanders with EnemyAI NavMesh, chases target when in range, and attacks on contact.
/// </summary>
public class SpikeEnemy : MonoBehaviour
{
    [Header("Target & Range")]
    public float chaseRange = 10f;
    public float attackRange = 2f;

    [Header("Combat")]
    public Animator animator;
    public float spikeEnemyHealth = 100f;
    public float spikeEnemyDamage = 2f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    // EnemyAI component for NavMesh wandering/chasing
    private EnemyAI enemyAI;
    private Transform player;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        player = FindObjectOfType<FPSController>().transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, player.position);

        // ATTACK when in attack range
        if (distanceToTarget <= attackRange && Time.time >= attackCooldown + lastAttackTime)
        {
            lastAttackTime = Time.time;
            StartCoroutine(AttackWithSpikes());
        }
        // CHASE when in chase range (override EnemyAI)
        else if (distanceToTarget <= chaseRange)
        {
            ChaseTarget();
        }
        // WANDER otherwise (let EnemyAI handle NavMesh wandering)
        else
        {
            Wander();
        }
    }

    private void ChaseTarget()
    {
        // Lock onto player for direct chase
        enemyAI.lockedOnPlayer = true;
        if (enemyAI.agent != null)
        {
            enemyAI.agent.SetDestination(player.position);
        }
    }

    private void Wander()
    {
        // Release lock, let EnemyAI wander
        enemyAI.lockedOnPlayer = false;
    }

    IEnumerator AttackWithSpikes()
    {
        // Stop movement during attack
        if (enemyAI.agent != null)
            enemyAI.agent.isStopped = true;

        animator.SetBool("isAttacking", true);
        AudioManager.Instance.PlaySound("Spike", false);

        yield return new WaitForSeconds(0.5f);

        PlayerFuelManager.Instance.currenthealth -= spikeEnemyDamage;
        animator.SetBool("isAttacking", false);

        // Resume movement
        if (enemyAI.agent != null)
            enemyAI.agent.isStopped = false;
    }

    public void TakeDamage(float damage = 3f)
    {
        spikeEnemyHealth -= damage;
        if (spikeEnemyHealth <= 0)
        {
            AudioManager.Instance.PlaySound("SlimeDeath", false);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Chase range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}