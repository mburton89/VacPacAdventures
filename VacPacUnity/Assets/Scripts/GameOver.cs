using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;

    [Header("References")]
    public GameObject gameOverCanvas;
    public TextMeshProUGUI totalScoreText;
    public GameObject pressAnyKeyText;   // Assign in Inspector

    private bool canRestart = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Allow restart only after "Press Any Key" appears
        if (canRestart && Input.anyKeyDown)
        {
            RestartScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            int totalScore = UIManager.Instance.playerHeightScore + UIManager.Instance.kills;
            ShowGameOver(totalScore);
        }
    }

    public void ShowGameOver(int totalScore)
    {
 
        gameOverCanvas.SetActive(true);

        totalScoreText.SetText("Total Score: " + totalScore);

        // Hide "Press Any Key" at first
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.SetActive(false);
        }

        // Start the 2-second delay before showing "Press Any Key"
        StartCoroutine(ShowPressAnyKeyAfterDelay(2f));
    }

    private IEnumerator ShowPressAnyKeyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.SetActive(true);
        }

        canRestart = true; // Now player can press any key to restart
    }

    public void RestartScene()
    {
        Time.timeScale = 1f; // In case you paused the game
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}