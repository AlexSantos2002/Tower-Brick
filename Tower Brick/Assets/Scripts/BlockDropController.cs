using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlockDropController : MonoBehaviour
{
    public float bottomLimitOffset = 0.5f;
    public LayerMask blockLayerMask;

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
            Destroy(gameObject);
        }
    }

    void DropBlock()
    {
        isDropped = true;
        transform.parent = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & blockLayerMask) != 0)
        {
            hasLanded = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Debug.Log("Bloco pousado com sucesso.");
        }
    }
}
