using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Sprite pauseSprite;
    private GameObject pauseObject;
    private bool isPaused = false;

    public float scaleMultiplier = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        CreatePauseObject();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseObject != null)
        {
            Destroy(pauseObject);
        }
    }

    void CreatePauseObject()
    {
        if (pauseObject != null)
        {
            return;
        }

        if (pauseSprite == null)
        {
            Debug.LogError("Pause sprite não foi atribuído no Inspector!");
            return;
        }

        pauseObject = new GameObject("PauseImage");

        SpriteRenderer spriteRenderer = pauseObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = pauseSprite;

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        pauseObject.transform.position = new Vector3(screenCenter.x, screenCenter.y, 0f);

        pauseObject.transform.localScale = Vector3.one * scaleMultiplier;

        spriteRenderer.sortingLayerName = "Blocks";
        spriteRenderer.sortingOrder = 100;

        Debug.Log("Objeto de pausa criado no centro da tela.");
    }
}