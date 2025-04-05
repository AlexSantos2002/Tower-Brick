using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlockDropController : MonoBehaviour
{
    public CameraManager cameraManager;
    public CraneSpawner spawner;
    public LayerMask blockLayerMask;
    public float bottomLimitOffset = 0.5f;

    public GameOverManager gameOverManager;

    [Header("Feedback Visual")]
    public Sprite perfectSprite;
    public Sprite goodSprite;
    public Sprite okSprite;

    private Rigidbody2D rb;
    private bool isDropped = false;
    private bool hasLanded = false;
    private float cameraBottomY;

    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        float camHeight = Camera.main.orthographicSize * 2f;
        cameraBottomY = Camera.main.transform.position.y - camHeight / 2f;
    }

    void Update()
    {
        if (!isDropped && Input.GetKeyDown(KeyCode.Space))
        {
            DropBlock();
        }

        if (isDropped && !hasLanded && transform.position.y < cameraBottomY - bottomLimitOffset)
        {
            Debug.Log("Bloco falhou e foi destruído.");
            gameOverManager?.TriggerGameOver();
            Destroy(gameObject);
        }
    }

    void DropBlock()
    {
        if (isDropped) return;

        isDropped = true;
        transform.parent = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDropped || hasLanded) return;

        if (((1 << collision.gameObject.layer) & blockLayerMask) != 0)
        {
            Bounds thisBounds = GetComponent<SpriteRenderer>().bounds;
            Bounds otherBounds = collision.collider.bounds;

            float overlapX = Mathf.Min(thisBounds.max.x, otherBounds.max.x) - Mathf.Max(thisBounds.min.x, otherBounds.min.x);
            float thisWidth = thisBounds.size.x;
            float overlapPercentage = overlapX / thisWidth;

            Debug.Log("Overlap %: " + (overlapPercentage * 100f).ToString("F1") + "%");

            if (overlapPercentage >= 0.5f)
            {
                hasLanded = true;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                transform.rotation = Quaternion.identity;

                Debug.Log("Bloco pousado com sucesso.");
                SFXManager.Instance?.PlayBlockLandingSound();

                Sprite feedbackSprite = null;
                string resultType = "";

                if (overlapPercentage >= 0.9f)
                {
                    feedbackSprite = perfectSprite;
                    resultType = "PERFECT";
                }
                else if (overlapPercentage >= 0.7f)
                {
                    feedbackSprite = goodSprite;
                    resultType = "GOOD";
                }
                else if (overlapPercentage >= 0.5f)
                {
                    feedbackSprite = okSprite;
                    resultType = "OK";
                }

                Debug.Log("Selected feedback sprite: " + (feedbackSprite ? feedbackSprite.name : "null"));

                if (feedbackSprite != null)
                {
                    GameObject spriteObj = new GameObject("LandingFeedbackSprite");
                    SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
                    sr.sprite = feedbackSprite;
                    sr.sortingLayerName = "Blocks";
                    sr.sortingOrder = 200;

                    Bounds bounds = GetComponent<SpriteRenderer>().bounds;
                    float marginX = 0.6f;
                    float marginY = 0.2f;
                    Vector3 offset = new Vector3(bounds.extents.x + marginX, -bounds.extents.y - marginY, -0.1f);
                    spriteObj.transform.position = transform.position + offset;

                    spriteObj.transform.localScale = Vector3.one * 0.15f;

                    Destroy(spriteObj, 2f);
                }

                if (!string.IsNullOrEmpty(resultType))
                {
                    ScoreManager.Instance?.AddScore(resultType);
                }

                cameraManager?.RegisterBlock(gameObject);
                spawner?.SpawnBlockOnCrane();
            }
            else
            {
                Debug.Log("Bloco mal pousado, vai cair.");
            }
        }
    }
}