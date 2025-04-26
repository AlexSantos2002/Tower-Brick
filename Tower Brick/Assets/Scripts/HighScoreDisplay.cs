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
    public float lineHeight = 0.7f;        // altura normal da linha (tamanho visual)
    public float linePadding = 0.1f;        // novo: padding extra para baixo entre linhas
    public float baseCharacterSpacing = 0.5f;
    public Vector2 startOffset = new Vector2(0f, 0f);

    private Transform uiParent;

    private const float numberScale = 0.5f;
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

        if (Camera.main == null)
        {
            Debug.LogError("[HighScoreDisplay] Nenhuma Camera.main encontrada!");
            return;
        }

        Vector3 topCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 10f));
        uiParent.position = new Vector3(topCenter.x + startOffset.x, topCenter.y + startOffset.y - 1, 0f);

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
        // Agora: altura real da linha + padding extra
        float yOffset = -(rankIndex * (lineHeight + linePadding));

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

        float totalWidth = (elements.Count - 1) * baseCharacterSpacing;
        float startX = -totalWidth / 2f;
        float xOffset = startX;

        foreach (var (sprite, scale) in elements)
        {
            CreateCharSprite(sprite, new Vector2(xOffset, yOffset), scale);
            xOffset += baseCharacterSpacing;
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
