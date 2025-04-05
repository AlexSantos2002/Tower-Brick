using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private string lastType = "";
    private int streak = 0;

    private ScoreDisplay scoreDisplay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        scoreDisplay = FindObjectOfType<ScoreDisplay>();
        UpdateScoreUI();
    }

    public void AddScore(string resultType)
    {
        int basePoints = 0;
        float multiplier = 1f;

        switch (resultType)
        {
            case "PERFECT":
                basePoints = 20;
                multiplier = 1.2f;
                break;
            case "GOOD":
                basePoints = 10;
                multiplier = 1.1f;
                break;
            case "OK":
                basePoints = 5;
                multiplier = 1f;
                break;
        }

        if (resultType == lastType && (resultType == "PERFECT" || resultType == "GOOD"))
        {
            streak++;
        }
        else
        {
            streak = 1;
            lastType = resultType;
        }

        float finalMultiplier = Mathf.Pow(multiplier, streak - 1);
        int pointsGained = Mathf.RoundToInt(basePoints * finalMultiplier);

        score += pointsGained;

        Debug.Log($"[{resultType}] Streak: {streak}, +{pointsGained} pontos (x{finalMultiplier:F2})");

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.SetScore(score);
        }
    }

    public int GetScore() => score;
}