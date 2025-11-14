using UnityEngine;

/// <summary>
/// Spawns N copies of a single prefab and blasts them outward in a spherical explosion.
/// </summary>
[RequireComponent(typeof(Transform))]
public class SlimeExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("Prefab that will be duplicated. Must have a Rigidbody component.")]
    [SerializeField] private GameObject prefab;

    [Tooltip("How many copies of the prefab to spawn.")]
    [SerializeField, Min(1)] private int numberOfPrefabsToInstantiate = 20;

    [Tooltip("Base force magnitude applied to each instance.")]
    [SerializeField] private float explosionForce = 800f;

    [Tooltip("Random variation added/subtracted from the base force.")]
    [SerializeField] private float forceVariation = 200f;

    [Tooltip("Optional small random offset so instances don't all start exactly at the same point.")]
    [SerializeField] private float spawnOffsetRadius = 0.3f;

    private void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("SphericalExplosion: No prefab assigned!", this);
            return;
        }

        Rigidbody prefabRb = prefab.GetComponent<Rigidbody>();
        if (prefabRb == null)
        {
            Debug.LogError($"SphericalExplosion: Prefab '{prefab.name}' has no Rigidbody!", this);
            return;
        }

        for (int i = 0; i < numberOfPrefabsToInstantiate; i++)
        {
            // 1. Random direction on a unit sphere
            Vector3 direction = Random.insideUnitSphere.normalized;

            // 2. Optional tiny offset so objects don't overlap at t=0
            Vector3 spawnPos = transform.position + direction * Random.Range(0f, spawnOffsetRadius);

            // 3. Instantiate
            GameObject instance = Instantiate(prefab, spawnPos, Random.rotation);
            Rigidbody rb = instance.GetComponent<Rigidbody>();

            // 4. Apply randomised force
            float finalForce = explosionForce + Random.Range(-forceVariation, forceVariation);
            rb.AddForce(direction * finalForce);

            // 5. (Optional) Add a little spin
            rb.AddTorque(Random.insideUnitSphere * finalForce * 0.1f);
        }

        // Remove the trigger object after spawning (optional)
        // Destroy(gameObject);
    }

    // Visualise the explosion radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, explosionForce * 0.002f); // rough visual aid
    }
}