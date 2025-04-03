using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite gameOverSprite;
    public Sprite playAgainButtonSprite;
    public Sprite mainMenuButtonSprite;

    private bool triggered = false;

    public void TriggerGameOver()
    {
        if (triggered) return;
        triggered = true;
        //Som do GameOver
        SFXManager.Instance?.PlayGameOverSound();

        // Cria o visual de Game Over (sprite na cena)
        GameObject gameOverObj = new GameObject("GameOver");
        SpriteRenderer sr = gameOverObj.AddComponent<SpriteRenderer>();
        sr.sprite = gameOverSprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 100;
        sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Vector3 camPos = Camera.main.transform.position;
        gameOverObj.transform.position = new Vector3(camPos.x, camPos.y + 3f, 0f);

        // Cria a UI com os botões
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        CreateUIButton(playAgainButtonSprite, new Vector2(0, 0), canvas.transform, () =>
        {
            SceneManager.LoadScene("GameScene");
        });

        CreateUIButton(mainMenuButtonSprite, new Vector2(0, -100), canvas.transform, () =>
        {
            SceneManager.LoadScene("Main Menu");
        });
    }

    private GameObject CreateUIButton(Sprite sprite, Vector2 anchoredPos, Transform parent, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(sprite.name + "Button");
        btnObj.transform.SetParent(parent);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 100);
        rt.anchoredPosition = anchoredPos;

        btnObj.transform.localScale = Vector3.one;

        Image img = btnObj.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        return btnObj;
    }
}