using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public static ScoreDisplay Instance { get; private set; }

    [Header("Font Sprites")]
    public Sprite[] numberSprites;
    public Sprite[] letterSprites;

    [Header("Layout Settings")]
    public float characterSpacing = 0.15f;
    public Vector2 screenOffset = new Vector2(0.5f, -0.5f);

    private Transform uiParent;
    private int currentScore = -1;
    private List<GameObject> characterObjects = new List<GameObject>();
    private readonly string label = "SCORE:";

    private const float characterScale = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        GameObject existing = GameObject.Find("ScoreUI");
        if (existing != null)
            Destroy(existing);

        GameObject parent = new GameObject("ScoreUI");
        uiParent = parent.transform;
        uiParent.SetParent(Camera.main.transform);
        uiParent.localPosition = Vector3.zero;

        SetScore(0);
    }

    void Update()
    {
        Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 10f));
        uiParent.position = topLeft + (Vector3)screenOffset;
    }

    public void SetScore(int newScore)
    {
        if (newScore == currentScore) return;

        currentScore = newScore;

        foreach (GameObject go in characterObjects)
            Destroy(go);
        characterObjects.Clear();

        float xOffset = 0f;
        float spacing = characterSpacing / characterScale;

        foreach (char c in label)
        {
            Sprite sprite = GetLetterSprite(c);
            if (sprite != null)
                characterObjects.Add(CreateCharSprite(sprite, new Vector2(xOffset, 0)));
            xOffset += spacing;
        }

        foreach (char c in newScore.ToString("D3"))
        {
            Sprite sprite = GetNumberSprite(c);
            if (sprite != null)
                characterObjects.Add(CreateCharSprite(sprite, new Vector2(xOffset, 0)));
            xOffset += spacing;
        }
    }

    private GameObject CreateCharSprite(Sprite sprite, Vector2 localPos)
    {
        GameObject obj = new GameObject(sprite.name);
        obj.transform.SetParent(uiParent);
        obj.transform.localPosition = localPos;

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 999;

        obj.transform.localScale = Vector3.one * characterScale;
        return obj;
    }

    private Sprite GetNumberSprite(char c)
    {
        int index = c - '0';
        return (index >= 0 && index < numberSprites.Length) ? numberSprites[index] : null;
    }

    private Sprite GetLetterSprite(char c)
    {
        c = char.ToUpper(c);
        switch (c)
        {
            case 'S': return letterSprites.Length > 0 ? letterSprites[0] : null;
            case 'C': return letterSprites.Length > 1 ? letterSprites[1] : null;
            case 'O': return letterSprites.Length > 2 ? letterSprites[2] : null;
            case 'R': return letterSprites.Length > 3 ? letterSprites[3] : null;
            case 'E': return letterSprites.Length > 4 ? letterSprites[4] : null;
            case ':': return letterSprites.Length > 5 ? letterSprites[5] : null;
        }
        return null;
    }
}
