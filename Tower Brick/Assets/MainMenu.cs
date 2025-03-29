using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void QuitGame(){
        Application.Quit();
        Debug.Log("O jogo foi fechado.");
    }
}
