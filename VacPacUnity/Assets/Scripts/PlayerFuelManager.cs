using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFuelManager : MonoBehaviour
{
    public static PlayerFuelManager Instance;

    [Header("Health & Damage")]
    public float currenthealth = 100f;
    public float maxHealth = 100f;

    public float currentFuel = 100f;
    public float maxFuel = 100f;

    [Header("UI")]
    public GameObject vacPacObject;
    public GameObject gameOverScreen;
    public Image fuelFill;
    public Image healthFill;

    // Invincibility tracking
    private bool _isInvincible = false;
    public bool IsInvincible => _isInvincible; // Optional: for other scripts to check

    private void Start()
    {
        Instance = this;
        currenthealth = maxHealth; // Ensure full health at start
    }

    private void Update()
    {
        fuelFill.fillAmount = currentFuel / maxFuel;

        if (gameOverScreen != null && currenthealth <= 0)
        {
            initiateGameOver();
        }
    }

    /// <summary>
    /// Public method to deal damage with built-in 2-second invincibility
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (_isInvincible) return;

        currenthealth -= damage;
        currenthealth = Mathf.Max(currenthealth, 0);

        // Start invincibility
        StartCoroutine(InvincibilityFrames(2f));

        healthFill.fillAmount = currenthealth / maxHealth;
    }

    private IEnumerator InvincibilityFrames(float duration)
    {
        _isInvincible = true;
        // Optional: flash effect here later
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }

    public void AddFuel(float fuelToAdd)
    {
        currentFuel += fuelToAdd;
        currentFuel = Mathf.Min(currentFuel, maxFuel);
    }

    private void initiateGameOver()
    {
        GetComponent<FPSController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverScreen.SetActive(true);
    }
}