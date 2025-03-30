using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(DelayedLoad("GameScene"));
    }

    public void Options()
    {
        StartCoroutine(DelayedLoad("Options"));
    }

    public void ToDoWarning()
    {
        StartCoroutine(DelayedLoad("ToDoWarning"));
    }

    public void Back()
    {
        StartCoroutine(DelayedBack());
    }

    public void QuitGame()
    {
        // Sem delay porque Application.Quit precisa ser imediato
        Application.Quit();
        Debug.Log("O jogo foi fechado.");
    }

    private IEnumerator DelayedLoad(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneLoader.LoadScene(sceneName);
    }

    private IEnumerator DelayedBack()
    {
        yield return new WaitForSeconds(0.5f);
        SceneLoader.LoadLastScene();
    }
}
