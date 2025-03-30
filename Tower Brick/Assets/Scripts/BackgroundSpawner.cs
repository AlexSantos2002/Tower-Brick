using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    [Header("Background Settings")]
    public Sprite backgroundSprite;
    public Vector3 position = new Vector3(0, 0, 10);
    public Vector3 scale = Vector3.one;
    public string sortingLayerName = "Background";
    public int orderInLayer = 0;

void Start()
{
    GameObject background = new GameObject("Background");
    SpriteRenderer sr = background.AddComponent<SpriteRenderer>();
    sr.sprite = backgroundSprite;
    sr.sortingLayerName = sortingLayerName;
    sr.sortingOrder = orderInLayer;

    float cameraHeight = Camera.main.orthographicSize * 2f;
    float cameraWidth = cameraHeight * Camera.main.aspect;

    Vector2 spriteSize = sr.sprite.bounds.size;

    float scale = Mathf.Max(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y);
    background.transform.localScale = new Vector3(scale, scale, 1f);

    float spriteHeightWorldUnits = spriteSize.y * scale;
    float bottomY = Camera.main.transform.position.y - cameraHeight / 2f;
    float adjustedY = bottomY + spriteHeightWorldUnits / 2f;

    background.transform.position = new Vector3(0, adjustedY, position.z);
}
}
