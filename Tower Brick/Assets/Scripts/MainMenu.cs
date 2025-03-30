using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneLoader.LoadScene("GameScene");
    }

    public void Options(){
        SceneLoader.LoadScene("Options");
    }

    public void ToDoWarning(){
        SceneLoader.LoadScene("ToDoWarning");
    }

    public void Back(){
        SceneLoader.LoadLastScene();
    }

    public void QuitGame(){
        Application.Quit();
        Debug.Log("O jogo foi fechado.");
    }
}