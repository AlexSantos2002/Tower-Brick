using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CraneSpawner : MonoBehaviour
{
    [Header("Crane Settings")]
    public Sprite craneSprite;
    public string sortingLayer = "Foreground";
    public int orderInLayer = 1;
    public float craneHeightInUnits = 1.2f;

    [Header("Block Settings")]
    public Sprite blockSprite;
    public float blockHeightInUnits = 1.5f;
    public string blockSortingLayer = "Blocks";
    public int blockOrderInLayer = 0;
    public float verticalSpacing = 0.01f;

    [Header("Movimento")]
    public float amplitude = 3f;
    public float speed = 2f;

    [Header("Game Over")]
    public Sprite gameOverSprite;
    public Sprite playAgainButtonSprite;
    public Sprite mainMenuButtonSprite;
    private bool gameOverTriggered = false;

    private GameObject crane;
    private GameObject currentBlock;
    private Vector3 startPos;

    void Start()
    {
        crane = new GameObject("Crane");
        SpriteRenderer craneSR = crane.AddComponent<SpriteRenderer>();
        craneSR.sprite = craneSprite;
        craneSR.sortingLayerName = sortingLayer;
        craneSR.sortingOrder = orderInLayer;

        float craneOriginalHeight = craneSR.sprite.bounds.size.y;
        float craneScale = craneHeightInUnits / craneOriginalHeight;
        crane.transform.localScale = new Vector3(craneScale, craneScale, 1f);

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float topY = Camera.main.transform.position.y + cameraHeight / 2f;
        float craneY = topY - (craneSR.sprite.bounds.size.y * craneScale) / 2f;

        crane.transform.position = new Vector3(0, craneY, -1f);
        startPos = crane.transform.position;

        SpawnBlockOnCrane();

        CameraManager manager = Object.FindFirstObjectByType<CameraManager>();
        if (manager != null)
        {
            manager.crane = crane.transform;
        }
    }

    void Update()
    {
        if (crane == null) return;

        float offsetX = Mathf.Sin(Time.time * speed) * amplitude;
        crane.transform.position = new Vector3(startPos.x + offsetX, startPos.y, startPos.z);

        if (currentBlock != null && currentBlock.transform.parent == crane.transform)
        {
            currentBlock.transform.position = crane.transform.position - new Vector3(0, GetOffsetToBlock(), 0);
        }
    }

    public void SpawnBlockOnCrane()
    {
        GameObject block = new GameObject("BlockOnCrane");

        block.layer = LayerMask.NameToLayer("Blocks");

        SpriteRenderer blockSR = block.AddComponent<SpriteRenderer>();
        blockSR.sprite = blockSprite;
        blockSR.sortingLayerName = blockSortingLayer;
        blockSR.sortingOrder = blockOrderInLayer;

        float blockOriginalHeight = blockSR.sprite.bounds.size.y;
        float blockScale = blockHeightInUnits / blockOriginalHeight;
        block.transform.localScale = new Vector3(blockScale, blockScale, 1f);

        float offset = GetOffsetToBlock();
        block.transform.position = crane.transform.position - new Vector3(0, offset, 0);
        block.transform.parent = crane.transform;

        block.AddComponent<BoxCollider2D>();

        BlockDropController dropController = block.AddComponent<BlockDropController>();
        dropController.blockLayerMask = LayerMask.GetMask("Blocks");
        dropController.spawner = this;
        dropController.cameraManager = Object.FindFirstObjectByType<CameraManager>();

        currentBlock = block;
    }

    float GetOffsetToBlock()
    {
        float craneHeight = crane.GetComponent<SpriteRenderer>().bounds.size.y * crane.transform.localScale.y;
        float blockHeight = blockSprite.bounds.size.y * (blockHeightInUnits / blockSprite.bounds.size.y);
        return (craneHeight / 2f) + (blockHeight / 2f) + verticalSpacing - 0.4f;
    }

    public void MoveCraneVertically(float amount)
    {
        crane.transform.position += new Vector3(0, amount, 0);
        startPos += new Vector3(0, amount, 0);
    }

    public void UpdateStartYSmooth(float newY)
    {
        startPos = new Vector3(startPos.x, newY, startPos.z);
    }

    public void TriggerGameOver()
    {
        if (gameOverTriggered) return;

        gameOverTriggered = true;

        GameObject gameOverObj = new GameObject("GameOver");
        SpriteRenderer sr = gameOverObj.AddComponent<SpriteRenderer>();
        sr.sprite = gameOverSprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 100;
        gameOverObj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Vector3 camPos = Camera.main.transform.position;
        gameOverObj.transform.position = new Vector3(camPos.x, camPos.y + 3f, 0f);

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

    private GameObject CreateUIButton(Sprite buttonSprite, Vector2 anchoredPosition, Transform parent, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject buttonObj = new GameObject(buttonSprite.name + "Button");
        buttonObj.transform.SetParent(parent);

        RectTransform rt = buttonObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 100);
        rt.anchoredPosition = anchoredPosition;

        buttonObj.transform.localScale = new Vector3(1f, 1f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        Image image = buttonObj.AddComponent<Image>();
        image.sprite = buttonSprite;
        image.preserveAspect = true;
        image.color = Color.white;

        button.onClick.AddListener(onClickAction);

        return buttonObj;
    }
}