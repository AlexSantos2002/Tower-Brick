using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    private ScoreDisplay scoreDisplay;

    private int perfectStreak = 0;
    private int goodStreak = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        yield return null;

        scoreDisplay = ScoreDisplay.Instance;

        if (scoreDisplay == null)
        {
            Debug.LogWarning("ScoreManager: ScoreDisplay.Instance não encontrado!");
        }
        else
        {
            scoreDisplay.SetScore(score);
        }
    }

    public void AddScore(string result)
    {
        float multiplier = 1f;
        int basePoints = 0;

        switch (result)
        {
            case "PERFECT":
                basePoints = 20;
                perfectStreak++;
                goodStreak = 0;
                multiplier = Mathf.Pow(1.2f, perfectStreak - 1);
                break;

            case "GOOD":
                basePoints = 10;
                goodStreak++;
                perfectStreak = 0;
                multiplier = Mathf.Pow(1.1f, goodStreak - 1);
                break;

            case "OK":
                basePoints = 5;
                perfectStreak = 0;
                goodStreak = 0;
                multiplier = 1f;
                break;
        }

        int addedPoints = Mathf.RoundToInt(basePoints * multiplier);
        score += addedPoints;

        Debug.Log($"[SCORE] Resultado: {result}, Pontos ganhos: {addedPoints}, Total: {score}");

        scoreDisplay?.SetScore(score);
    }

    public int GetScore()
    {
        return score;
    }
}
