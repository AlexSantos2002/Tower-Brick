using UnityEngine;

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

    [Header("Referências")]
    public GameOverManager gameOverManager; // << novo campo

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
        dropController.gameOverManager = gameOverManager; // <- nova referência

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
}