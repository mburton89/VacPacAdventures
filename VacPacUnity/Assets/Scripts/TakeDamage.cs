using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public GameObject healthBarContainer;
    public Image healthBar; // Assign a UI Image with Fill Method = Horizontal and origin Left or Right

    [Header("Death Effect")]
    public GameObject explosionParticlePrefab; // Assign your explosion Particle System prefab

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        healthBarContainer.SetActive(false);
    }

    public void ApplyDamage(float damage)
    {
        healthBarContainer.SetActive(true);

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        // Spawn explosion effect at this object's position
        if (explosionParticlePrefab != null)
        {
            Instantiate(explosionParticlePrefab, transform.position, transform.rotation);
        }

        UIManager.Instance.AddKill();

        // Destroy the object (you can also use object pooling instead)
        Destroy(gameObject);
    }

    // Optional: Visual debug in editor
    private void OnValidate()
    {
        if (healthBar != null)
        {
            healthBar.type = Image.Type.Filled;
            healthBar.fillMethod = Image.FillMethod.Horizontal;
        }
    }
}