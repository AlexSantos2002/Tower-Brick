using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private List<int> highScores = new List<int>();
    private string filePath;

    private const int maxScores = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "scores.csv");
        LoadScores();
    }

    public void RegisterScore(int score)
    {
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a));

        if (highScores.Count > maxScores)
            highScores.RemoveAt(highScores.Count - 1);

        SaveScores();
    }

    public List<int> GetScores()
    {
        return new List<int>(highScores);
    }

    private void LoadScores()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            highScores.Clear();

            foreach (string line in lines)
            {
                if (int.TryParse(line, out int score))
                {
                    highScores.Add(score);
                }
            }

            highScores.Sort((a, b) => b.CompareTo(a));
        }
        else
        {
            Debug.Log("[HighScoreManager] Nenhum arquivo de scores encontrado, criando novo...");
            SaveScores();
        }
    }

    private void SaveScores()
    {
        List<string> lines = new List<string>();

        foreach (int score in highScores)
        {
            lines.Add(score.ToString());
        }

        File.WriteAllLines(filePath, lines.ToArray());
    }
}
