using UnityEngine;
public class CraneSpawner : MonoBehaviour
{
    public Sprite craneSprite;
    public string sortingLayer = "Foreground";
    public int orderInLayer = 1;
    [Header("Tamanho da Grua")]
    public float craneHeightInUnits = 1.2f;

    [Header("Movimento Horizontal")]
    public float amplitude = 3f;
    public float speed = 2f;
    private GameObject crane;
    private Vector3 startPos;

    void Start()
    {
        crane = new GameObject("Crane");
        SpriteRenderer sr = crane.AddComponent<SpriteRenderer>();
        sr.sprite = craneSprite;
        sr.sortingLayerName = sortingLayer;
        sr.sortingOrder = orderInLayer;
        float originalHeight = sr.sprite.bounds.size.y;
        float scale = craneHeightInUnits / originalHeight;
        crane.transform.localScale = new Vector3(scale, scale, 1f);
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float topY = Camera.main.transform.position.y + cameraHeight / 2f;
        float finalY = topY - (sr.sprite.bounds.size.y * scale) / 2f;
        crane.transform.position = new Vector3(0, finalY, -1f);
        startPos = crane.transform.position;
    }

    void Update()
    {
        if (crane == null) return;

        float offsetX = Mathf.Sin(Time.time * speed) * amplitude;
        crane.transform.position = new Vector3(startPos.x + offsetX, startPos.y, startPos.z);
    }
}
