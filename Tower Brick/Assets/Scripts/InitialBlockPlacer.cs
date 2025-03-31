using UnityEngine;

public class InitialBlockPlacer : MonoBehaviour
{
    public Sprite blockSprite;
    public string sortingLayer = "Blocks";
    public int orderInLayer = 0;

    [Header("Scale Settings")]
    public float targetBlockHeightInUnits = 1.5f;

    void Start()
    {
        GameObject block = new GameObject("BaseBlock");
        SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = blockSprite;
        sr.sortingLayerName = sortingLayer;
        sr.sortingOrder = orderInLayer;

        float originalHeight = sr.sprite.bounds.size.y;
        float scale = targetBlockHeightInUnits / originalHeight;
        block.transform.localScale = new Vector3(scale, scale, 1f);

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float bottomY = Camera.main.transform.position.y - cameraHeight / 2f;
        float finalY = bottomY + (originalHeight * scale) / 2f;

        block.transform.position = new Vector3(0, finalY, 0);
    }
}