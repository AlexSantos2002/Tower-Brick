using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite gameOverSprite;
    public Sprite playAgainButtonSprite;
    public Sprite mainMenuButtonSprite;
    public Sprite highscoreButtonSprite;

    [Header("Button Settings")]
    public Vector2 buttonSize = new Vector2(200, 100);
    public Vector2 highscoreButtonSize = new Vector2(150, 80);
    public float spacing = 90f;
    public float topButtonY = 0f;

    private bool triggered = false;

    public void TriggerGameOver()
    {
        if (triggered) return;
        triggered = true;

        SFXManager.Instance?.PlayGameOverSound();

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
        HighScoreManager.Instance?.RegisterScore(finalScore);

        CameraManager cam = Object.FindFirstObjectByType<CameraManager>();
        if (cam != null)
        {
            cam.TriggerCollapse();
        }

        GameObject gameOverObj = new GameObject("GameOver");
        SpriteRenderer sr = gameOverObj.AddComponent<SpriteRenderer>();
        sr.sprite = gameOverSprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 100;
        sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Vector3 camPos = Camera.main.transform.position;
        gameOverObj.transform.position = new Vector3(camPos.x, camPos.y + 3f, 0f);

        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        float currentY = topButtonY;

        CreateUIButton(playAgainButtonSprite, new Vector2(0, currentY), canvas.transform, buttonSize, () =>
        {
            ScoreManager.Instance?.ResetScore();
            SceneManager.LoadScene("GameScene");
        });

        currentY -= spacing;

        CreateUIButton(mainMenuButtonSprite, new Vector2(0, currentY), canvas.transform, buttonSize, () =>
        {
            SceneManager.LoadScene("Main Menu");
        });

        currentY -= spacing;

        CreateUIButton(highscoreButtonSprite, new Vector2(0, currentY), canvas.transform, highscoreButtonSize, () =>
        {
            SceneManager.LoadScene("HighscoreScene");
        });
    }

    private GameObject CreateUIButton(Sprite sprite, Vector2 anchoredPos, Transform parent, Vector2 sizeDelta, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(sprite.name + "Button");
        btnObj.transform.SetParent(parent);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        Image img = btnObj.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        return btnObj;
    }
}