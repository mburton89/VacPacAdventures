using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamagePlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("How much health to remove when hitting the player")]
    public float damageAmount = 25f;

    [Tooltip("Can this object damage the player multiple times? (false = one-time use)")]
    public bool canDamageMultipleTimes = true;

    [Tooltip("Optional tag check - leave empty to damage any PlayerFuelManager")]
    public string playerTag = "Player";

    private bool _hasDamagedOnce = false;

    private void Reset()
    {
        // Auto-setup THIS collider as trigger (child trigger will handle detection)
        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    // These methods will ONLY fire on objects with Collider components
    // Child trigger colliders will call these on the parent DamagePlayer!
    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject go)
    {
        // Prevent multiple damage if disabled
        if (!canDamageMultipleTimes && _hasDamagedOnce) return;

        // Optional tag check
        if (!string.IsNullOrEmpty(playerTag) && !go.CompareTag(playerTag))
            return;

        var player = go.GetComponentInParent<PlayerFuelManager>();
        if (player != null)
        {
            Debug.Log($"DamagePlayer hit! Dealing {damageAmount} damage", this);
            player.TakeDamage(damageAmount);
            _hasDamagedOnce = true;
        }
    }

    // Reset for object pooling/reuse
    private void OnEnable()
    {
        if (!canDamageMultipleTimes)
            _hasDamagedOnce = false;
    }
}