using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Sprite pauseSprite;  
    public Sprite secondarySprite;  
    private GameObject pauseObject;  
    private GameObject secondaryObject;  
    private bool isPaused = false;
    private float blinkTimer = 0f;
    private bool isSecondaryVisible = true;

    public float scaleMultiplier = 1f;  
    public float secondaryScale = 0.8f;  
    public float pauseImageYOffset = 2f;

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

        if (isPaused && secondaryObject != null)
        {
            blinkTimer += Time.unscaledDeltaTime;
            
            if (blinkTimer >= 1f)
            {
                blinkTimer = 0f;
                isSecondaryVisible = !isSecondaryVisible;
                secondaryObject.SetActive(isSecondaryVisible);
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        CreatePauseObject();
        CreateSecondaryObject();
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseObject != null) Destroy(pauseObject);
        if (secondaryObject != null) Destroy(secondaryObject);
    }

    void CreatePauseObject()
    {
        if (pauseObject != null) return;

        if (pauseSprite == null)
        {
            Debug.LogError("Pause sprite não foi atribuído no Inspector!");
            return;
        }

        pauseObject = new GameObject("PauseImage");
        SpriteRenderer spriteRenderer = pauseObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = pauseSprite;

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        pauseObject.transform.position = new Vector3(screenCenter.x, screenCenter.y + pauseImageYOffset, 0f);

        pauseObject.transform.localScale = Vector3.one * scaleMultiplier;
        spriteRenderer.sortingLayerName = "Blocks";
        spriteRenderer.sortingOrder = 100;
    }

    void CreateSecondaryObject()
    {
        if (secondaryObject != null || secondarySprite == null) return;

        secondaryObject = new GameObject("SecondaryImage");
        SpriteRenderer secondaryRenderer = secondaryObject.AddComponent<SpriteRenderer>();
        secondaryRenderer.sprite = secondarySprite;

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        secondaryObject.transform.position = screenCenter;

        secondaryObject.transform.localScale = Vector3.one * secondaryScale;
        secondaryRenderer.sortingLayerName = "Blocks";
        secondaryRenderer.sortingOrder = 101;

        blinkTimer = 0f;
        isSecondaryVisible = true;
        secondaryObject.SetActive(true);
    }
}