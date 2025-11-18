using UnityEngine;

/// <summary>
/// Spawns slime balls upward at regular intervals, like a fountain.
/// Destroys them after a set lifetime. Runs forever.
/// </summary>
public class SlimeFountain : MonoBehaviour
{
    [Header("Slime Ball Settings")]
    [Tooltip("The slime ball prefab to spawn")]
    public GameObject slimeBallPrefab;

    [Tooltip("How often to spawn a new slime ball (in seconds)")]
    public float spawnInterval = 1.5f;

    [Tooltip("Upward force applied to each slime ball")]
    public float upwardForce = 12f;

    [Tooltip("How long each slime ball lives before being destroyed (in seconds)")]
    public float slimeLifetime = 5f;

    [Header("Spawn Offset (Optional)")]
    [Tooltip("Spawn position offset from the fountain's position")]
    public Vector3 spawnOffset = Vector3.up * 0.5f;

    private void Start()
    {
        // Start spawning immediately and repeat forever
        InvokeRepeating(nameof(SpawnSlimeBall), 0f, spawnInterval);
    }

    private void SpawnSlimeBall()
    {
        if (slimeBallPrefab == null)
        {
            Debug.LogError("SlimeFountain: No slimeBallPrefab assigned!", this);
            return;
        }

        // Spawn position
        Vector3 spawnPos = transform.position + transform.TransformDirection(spawnOffset);

        // Instantiate the slime ball
        GameObject slime = Instantiate(slimeBallPrefab, spawnPos, Quaternion.identity);

        // Optional: Make it a child of the fountain for organization
        slime.transform.SetParent(transform);

        // Add upward force
        Rigidbody rb = slime.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset any existing velocity
            rb.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning("Slime ball prefab has no Rigidbody! Adding force will do nothing.", slime);
        }

        // Destroy after lifetime
        Destroy(slime, slimeLifetime);
    }

    // Optional: Visualize spawn point in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 spawnPos = transform.position + transform.TransformDirection(spawnOffset);
        Gizmos.DrawWireSphere(spawnPos, 0.2f);
        Gizmos.DrawRay(spawnPos, Vector3.up * 2f);
    }
}