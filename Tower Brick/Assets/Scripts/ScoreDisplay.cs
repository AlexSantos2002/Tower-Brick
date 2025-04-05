using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Font Sprites")]
    public Sprite[] numberSprites;
    public Sprite[] letterSprites;

    [Header("Layout Settings")]
    public float characterSpacing = 0.15f;
    public Vector2 startPosition = new Vector2(-5f, 3.2f);
    public Transform uiParent;

    private int currentScore = 0;
    private List<GameObject> characterObjects = new List<GameObject>();
    private readonly string label = "SCORE:";

    void Start()
    {
        if (uiParent == null)
        {
            GameObject parent = new GameObject("ScoreUI");
            uiParent = parent.transform;
            uiParent.SetParent(Camera.main.transform);
        }

        RenderLabel();
        RenderScore(currentScore);
    }

    void RenderLabel()
    {
        float xOffset = 0f;

        foreach (char c in label)
        {
            Sprite sprite = GetLetterSprite(c);
            if (sprite == null) continue;

            GameObject go = CreateCharSprite(sprite, startPosition + new Vector2(xOffset, 0));
            characterObjects.Add(go);
            xOffset += characterSpacing;
        }
    }

    void RenderScore(int score)
    {
        string scoreStr = score.ToString("D3");

        float xOffset = label.Length * characterSpacing;

        foreach (char c in scoreStr)
        {
            Sprite sprite = GetNumberSprite(c);
            if (sprite == null) continue;

            GameObject go = CreateCharSprite(sprite, startPosition + new Vector2(xOffset, 0));
            characterObjects.Add(go);
            xOffset += characterSpacing;
        }
    }

    GameObject CreateCharSprite(Sprite sprite, Vector2 localPos)
    {
        GameObject obj = new GameObject(sprite.name);
        obj.transform.SetParent(uiParent);
        obj.transform.localPosition = localPos;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 999;

        obj.transform.localScale = Vector3.one * 2.5f;

        return obj;
    }

    Sprite GetNumberSprite(char c)
    {
        int index = c - '0';
        if (index >= 0 && index <= 9 && index < numberSprites.Length)
            return numberSprites[index];

        return null;
    }

    Sprite GetLetterSprite(char c)
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

    public void SetScore(int newScore)
    {
        foreach (GameObject go in characterObjects)
            Destroy(go);
        characterObjects.Clear();

        RenderLabel();
        RenderScore(newScore);
        currentScore = newScore;
    }
}