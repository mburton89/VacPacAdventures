using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Clean EnemyAI for NavMesh-based wandering and player chasing.
/// Handles detection via public methods (call from triggers/colliders).
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent; // Auto-assigned in Awake if null

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 3f;

    [Header("Detection")]
    [SerializeField] private float stoppingDistance = 1.5f;

    // Private state
    private Transform player;
    private float timer;
    private bool playerDetected;
    public bool lockedOnPlayer;

    private void Awake()
    {
        // Auto-assign agent
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Set stopping distance
        if (agent != null)
            agent.stoppingDistance = stoppingDistance;
    }

    private void Start()
    {
        player = FindObjectOfType<FPSController>().transform;
        timer = wanderInterval;
    }

    private void Update()
    {
        // PRIORITY 1: Locked chase (collision-based)
        if (lockedOnPlayer && player != null)
        {
            agent.SetDestination(player.position);
            return;
        }

        // PRIORITY 2: Detected chase (trigger-based)
        if (playerDetected && player != null)
        {
            agent.SetDestination(player.position);
        }
        // PRIORITY 3: Wander
        else
        {
            Wander();
        }
    }

    private void Wander()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f || agent.remainingDistance < stoppingDistance + 0.1f)
        {
            Vector3 newPos = GetRandomNavPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = wanderInterval;
        }
    }

    /// <summary> Call from trigger/collision when player enters range </summary>
    public void OnPlayerEnter(Transform playerTransform)
    {
        playerDetected = true;
        player = playerTransform;
    }

    /// <summary> Call from trigger when player exits range </summary>
    public void OnPlayerExit()
    {
        playerDetected = false;
        // Force immediate new wander point
        timer = 0f;
    }

    /// <summary> Call from OnCollisionEnter with Player </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            lockedOnPlayer = true;
            player = collision.collider.transform;
        }
    }

    /// <summary> Utility: Get random valid NavMesh point </summary>
    public static Vector3 GetRandomNavPoint(Vector3 origin, float distance)
    {
        Vector3 randomPoint = origin + Random.insideUnitSphere * distance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return origin; // Fallback
    }
}