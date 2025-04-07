using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundSpawner : MonoBehaviour
{
    public static BackgroundSpawner Instance;

    [Header("Sprites")]
    public Sprite backgroundSprite;
    public Sprite[] skySprites;           // Todos os céus, usa os últimos 5
    public Sprite loopSkySprite;          // Céu que se repete

    [Header("Configuração")]
    public int loopCount = 10;
    public Vector3 position = new Vector3(0, 0, 10);
    public string sortingLayerName = "Background";
    public int orderInLayer = 0;

    private Transform backgroundParent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegenerateBackground();
    }

    public void RegenerateBackground()
    {
        // Apaga todos os backgrounds anteriores
        if (backgroundParent != null)
        {
            Destroy(backgroundParent.gameObject);
        }

        backgroundParent = new GameObject("BackgroundContainer").transform;
        backgroundParent.SetParent(transform);

        if (backgroundSprite == null || loopSkySprite == null)
        {
            Debug.LogError("Sprites obrigatórios não estão atribuídos.");
            return;
        }

        float currentY = CreateSpriteInstance("Background", backgroundSprite, 0);

        // Usa os 5 últimos elementos (pulando o primeiro)
        int startIndex = Mathf.Max(1, skySprites.Length - 5);
        for (int i = startIndex; i < skySprites.Length; i++)
        {
            currentY = CreateSpriteInstance($"Sky_{i}", skySprites[i], currentY);
        }

        for (int i = 0; i < loopCount; i++)
        {
            currentY = CreateSpriteInstance($"SkyLoop_{i}", loopSkySprite, currentY);
        }
    }

    float CreateSpriteInstance(string name, Sprite sprite, float startY)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(backgroundParent);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = orderInLayer;

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        Vector2 spriteSize = sprite.bounds.size;
        float scale = Mathf.Max(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y);
        go.transform.localScale = new Vector3(scale, scale, 1f);

        float spriteHeightWorldUnits = spriteSize.y * scale;
        float y = (startY == 0)
            ? Camera.main.transform.position.y - cameraHeight / 2f + spriteHeightWorldUnits / 2f
            : startY + spriteHeightWorldUnits / 2f;

        go.transform.position = new Vector3(0, y, position.z);

        return y + spriteHeightWorldUnits / 2f;
    }
}
