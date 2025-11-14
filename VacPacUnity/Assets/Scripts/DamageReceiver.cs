using UnityEngine;

/// <summary>
/// DamageReceiver - Attach to CHILD of enemy with NavMeshAgent.
/// Forwards ALL particle collisions (no tag/layer check) to parent's TakeDamage.
/// Also handles physics bullets (optional, with tag).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DamageReceiver : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageMultiplier = 1f;

    [Header("Bullet Settings (Optional)")]
    [SerializeField] private bool destroyBulletOnHit = true;
    [SerializeField] private string bulletTag = "Bullet";

    private TakeDamage parentDamage;

    private void Awake()
    {
        parentDamage = GetComponentInParent<TakeDamage>();
        if (parentDamage == null)
        {
            Debug.LogError("DamageReceiver: No TakeDamage found in parent!", this);
        }
    }

    // PHYSICS BULLETS / PROJECTILES (still uses tag for safety)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(bulletTag))
        {
            float damage = 10f * damageMultiplier;
            parentDamage?.ApplyDamage(damage);

            if (destroyBulletOnHit)
                Destroy(collision.gameObject);
        }
    }

    // PARTICLE SYSTEMS: ANY particle with GiveDamage component hurts it
    private void OnParticleCollision(GameObject other)
    {
        GiveDamage giveDamage = other.GetComponent<GiveDamage>();
        if (giveDamage != null)
        {
            float damage = giveDamage.damagePerHit * damageMultiplier;
            parentDamage?.ApplyDamage(damage);
        }
    }

    // Optional: Visualize in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is SphereCollider sphere)
                Gizmos.DrawWireSphere(transform.position, sphere.radius * transform.lossyScale.x);
            else
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}