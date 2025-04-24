using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private List<int> highScores = new List<int>();

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

    public void RegisterScore(int score)
    {
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a)); // Ordem decrescente

        Debug.Log("[HIGH SCORES]");
        for (int i = 0; i < highScores.Count; i++)
        {
            Debug.Log($"#{i + 1}: {highScores[i]}");
        }
    }

    public List<int> GetScores()
    {
        return new List<int>(highScores);
    }
}
