using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;

    private string currentComboType = "";
    private int comboMultiplier = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddScore(string resultType)
    {
        int basePoints = GetBasePoints(resultType);

        if (resultType.ToUpper() == currentComboType.ToUpper())
        {
            // Mesmo tipo que o anterior → dobra o multiplicador
            comboMultiplier *= 2;
        }
        else
        {
            // Tipo diferente → novo combo
            currentComboType = resultType.ToUpper();
            comboMultiplier = 1;
        }

        int totalPoints = basePoints * comboMultiplier;
        score += totalPoints;

        Debug.Log($"[SCORE] {resultType} → {basePoints} x{comboMultiplier} = {totalPoints} | Total: {score}");

        ScoreDisplay.Instance?.SetScore(score);
    }

    private int GetBasePoints(string resultType)
    {
        switch (resultType.ToUpper())
        {
            case "PERFECT": return 50;
            case "GOOD": return 20;
            case "OK": return 5;
            default: return 0;
        }
    }

    public void ResetScore()
    {
        score = 0;
        comboMultiplier = 1;
        currentComboType = "";
        ScoreDisplay.Instance?.SetScore(score);
    }

    public int GetScore()
    {
        return score;
    }
}
