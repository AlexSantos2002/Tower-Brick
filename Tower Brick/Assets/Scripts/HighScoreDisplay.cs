using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [Header("Font Sprites")]
    public Sprite[] numberSprites;
    public Sprite[] rankSprites;
    public Sprite colonSprite;

    [Header("Layout Settings")]
    public float characterSpacing = 0.3f;
    public Vector2 startOffset = new Vector2(0f, 0f);
    public float lineSpacing = 0.6f;

    [Header("Background Settings")]
    public Sprite backgroundSprite;
    public float backgroundScaleMultiplier = 0.55f;
    public Vector2 backgroundOffset = new Vector2(0f, 0f);

    private Transform uiParent;
    private Transform backgroundParent;

    private const float numberScale = 0.4f;
    private const float rankScale = numberScale * 0.3f;

    private IEnumerator Start()
    {
        while (HighScoreManager.Instance == null)
            yield return null;

        CreateDisplay();
    }

    private void CreateDisplay()
    {
        GameObject parent = new GameObject("HighScoreUI");
        uiParent = parent.transform;
        uiParent.SetParent(Camera.main.transform);
        uiParent.localPosition = Vector3.zero;

        GameObject bgParent = new GameObject("BackgroundUI");
        backgroundParent = bgParent.transform;
        backgroundParent.SetParent(Camera.main.transform);
        backgroundParent.localPosition = Vector3.zero;

        if (Camera.main == null)
        {
            Debug.LogError("[HighScoreDisplay] Nenhuma Camera.main encontrada!");
            return;
        }

        Vector3 topCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 10f));
        uiParent.position = new Vector3(topCenter.x + startOffset.x, topCenter.y + startOffset.y - 2f, 0f);

        List<int> scores = HighScoreManager.Instance.GetScores();

        if (scores.Count == 0)
        {
            Debug.LogWarning("Nenhum score registrado para exibir.");
            return;
        }

        for (int i = 0; i < Mathf.Min(10, scores.Count); i++)
        {
            CreateScoreLine(i, scores[i]);
        }
    }

    private void CreateScoreLine(int rankIndex, int score)
    {
        float yOffset = -(rankIndex * lineSpacing);

        List<(Sprite sprite, float scale)> elements = new List<(Sprite, float)>();

        if (rankIndex < rankSprites.Length)
            elements.Add((rankSprites[rankIndex], rankScale));

        if (colonSprite != null)
            elements.Add((colonSprite, numberScale));

        foreach (char c in score.ToString())
        {
            Sprite numberSprite = GetNumberSprite(c);
            if (numberSprite != null)
                elements.Add((numberSprite, numberScale));
        }

        float totalWidth = (elements.Count - 1) * characterSpacing;
        float startX = -totalWidth / 2f;
        float xOffset = startX;

        if (rankIndex == 0 && backgroundSprite != null)
        {
            CreateBackgroundSprite(new Vector2(0f, yOffset), Vector2.one * backgroundScaleMultiplier);
        }

        foreach (var (sprite, scale) in elements)
        {
            CreateCharSprite(sprite, new Vector2(xOffset, yOffset), scale);
            xOffset += characterSpacing;
        }
    }

    private void CreateBackgroundSprite(Vector2 localPos, Vector2 scale)
    {
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(backgroundParent);

        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = backgroundSprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 998;

        bg.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        if (sr.sprite != null)
        {
            Vector2 spriteSize = sr.sprite.bounds.size;
            Vector2 spriteCenterOffset = new Vector2(spriteSize.x * scale.x * 0.5f, spriteSize.y * scale.y * 0.5f);

            Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.65f, 1f, 10f));
            bg.transform.position = new Vector3(centerPos.x + backgroundOffset.x, centerPos.y + backgroundOffset.y - 1f, 0f);
            bg.transform.localPosition -= new Vector3(spriteCenterOffset.x, spriteCenterOffset.y, 0f);
        }
        else
        {
            bg.transform.localPosition = localPos;
        }
    }

    private GameObject CreateCharSprite(Sprite sprite, Vector2 localPos, float scale)
    {
        GameObject obj = new GameObject(sprite.name);
        obj.transform.SetParent(uiParent);
        obj.transform.localPosition = localPos;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Blocks";
        sr.sortingOrder = 999;

        obj.transform.localScale = Vector3.one * scale;
        return obj;
    }

    private Sprite GetNumberSprite(char c)
    {
        int index = c - '0';
        return (index >= 0 && index < numberSprites.Length) ? numberSprites[index] : null;
    }
}
