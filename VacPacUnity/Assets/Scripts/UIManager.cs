using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public int playerHeightScore;
    public int highScore;
    public int kills;

    public TextMeshProUGUI playerHeightScoreTMP;
    public TextMeshProUGUI playerKillsScoreTMP;
    public TextMeshProUGUI timerTMP;

    public KeyCode resetScoreKey = KeyCode.R;
    public KeyCode debugGameOverKey = KeyCode.G;
    public KeyCode debugTimerAdd = KeyCode.T;

    [Header("Timer Settings")]
    public float timeLeft = 60f;           // Starting time in seconds
    private float displayTime;             // For clean display
    public bool timerIsCounting = false;   // Set to true when game starts

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore", 0);
        displayTime = timeLeft;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        // Reset high score
        if (Input.GetKeyDown(resetScoreKey))
            ResetBestScore();

        // Debug keys
        if (Input.GetKeyDown(debugGameOverKey))
            GameOver.Instance.ShowGameOver(playerHeightScore + kills);

        if (Input.GetKeyDown(debugTimerAdd))
            AddToTimer(10f);

        // Update height score
        UpdateHeightScore();

        // Run timer only if active
        if (timerIsCounting && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                timerIsCounting = false;
                GameOver.Instance.ShowGameOver(highScore + kills);
            }
        }
    }

    private void UpdateHeightScore()
    {
        float playerY = PlayerFuelManager.Instance.transform.position.y;

        if (playerY > 0 && playerY < 61)
        {
            int newScore = Mathf.FloorToInt(playerY / 2f);

            if (newScore > playerHeightScore)
            {
                playerHeightScore = newScore;
                playerHeightScoreTMP.text = playerHeightScore.ToString();

                if (playerHeightScore > highScore)
                {
                    highScore = playerHeightScore;
                    PlayerPrefs.SetInt("highScore", highScore);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        displayTime = Mathf.Ceil(timeLeft); // Shows 60, 59, 58...
        timerTMP.text = displayTime.ToString("F0"); // Removes decimal
    }

    public void StartTimer()
    {
        timerIsCounting = true;
    }

    public void StopTimer()
    {
        timerIsCounting = false;
    }

    public void AddToTimer(float seconds)
    {
        timeLeft += seconds;
        UpdateTimerDisplay();
    }

    public void AddTimeToFinalScore()
    {
        int bonus = Mathf.CeilToInt(timeLeft);
        playerHeightScore += bonus;
        playerHeightScoreTMP.text = "Score: " + playerHeightScore;

        if (playerHeightScore > highScore)
        {
            highScore = playerHeightScore;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void AddKill()
    {
        kills++;
        playerKillsScoreTMP.text = kills.ToString();
    }

    private void ResetBestScore()
    {
        PlayerPrefs.SetInt("highScore", 0);
        highScore = 0;
        PlayerPrefs.Save();
        Debug.Log("High Score Reset!");
    }
}