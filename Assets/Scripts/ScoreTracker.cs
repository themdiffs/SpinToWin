using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance { get; private set; }

    public int score = 0;
    public float timeRemaining = 30f;
    public float elapsed = 0f;

    [Header("Optional UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    private bool isRunning = true;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isRunning)
            return;

        elapsed += Time.deltaTime;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            Debug.Log($"Time's up, final score: {score}");
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (!isRunning) 
            return;

        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (timerText != null) timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }
}