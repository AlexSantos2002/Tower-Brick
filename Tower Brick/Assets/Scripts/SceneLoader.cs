using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Stack<string> sceneHistory = new Stack<string>();

    public static void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Só adiciona se a cena atual não for igual à última da pilha
        if (sceneHistory.Count == 0 || sceneHistory.Peek() != currentScene)
        {
            sceneHistory.Push(currentScene);
        }

        SceneManager.LoadSceneAsync(sceneName);
    }

    public static void LoadLastScene()
    {
        if (sceneHistory.Count > 0)
        {
            string lastScene = sceneHistory.Pop();

            // 🔥 Limpa a pilha se estamos voltando para o menu principal
            if (lastScene == "Main Menu")
            {
                ClearHistory();
            }

            SceneManager.LoadSceneAsync(lastScene);
        }
        else
        {
            Debug.Log("Histórico vazio. Indo para o menu principal.");
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    public static void ClearHistory()
    {
        sceneHistory.Clear();
        Debug.Log("Histórico de cenas limpo.");
    }
}