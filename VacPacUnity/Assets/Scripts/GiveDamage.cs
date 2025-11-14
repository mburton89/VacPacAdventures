using UnityEngine;

public class GiveDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerHit = 10f;
    public float damageRate = 0.1f; // How often damage is applied (prevents insane DPS from particles)

    private float lastDamageTime;

    private void OnParticleCollision(GameObject other)
    {
        // Only apply damage at a controlled rate
        if (Time.time - lastDamageTime < damageRate)
            return;

        TakeDamage takeDamage = other.GetComponent<TakeDamage>();
        if (takeDamage != null)
        {
            takeDamage.ApplyDamage(damagePerHit);
            lastDamageTime = Time.time;
        }
    }
}