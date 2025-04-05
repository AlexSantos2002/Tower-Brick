using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    [Header("Configurações de Altura")]
    public float topMargin = 2f;
    public float cameraMoveAmount = 0.1f;
    public float cameraMoveSpeed = 2f;
    public float destroyBelowY = -100f;

    [Header("Referências")]
    public Transform mainCamera;
    public Transform crane;

    [Header("Tremor do Prédio")]
    public float baseShakeAmplitude = 0.25f;
    public float maxShakeAmplitude = 1.5f;
    public float shakeFrequency = 3f;

    private float shakeTimer = 0f;
    private float currentShakeAmplitude = 0f;

    private List<GameObject> placedBlocks = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    private bool isMovingUp = false;
    private Vector3 targetCameraPosition;
    private float craneTargetY = 0f;

    private bool collapseTriggered = false;

    void Start()
    {
        if (mainCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null) mainCamera = cam.transform;
        }
    }

    public void RegisterBlock(GameObject block)
    {
        if (!placedBlocks.Contains(block))
        {
            placedBlocks.Add(block);
            originalPositions[block] = block.transform.position;
        }

        float blockTopY = block.transform.position.y + block.GetComponent<SpriteRenderer>().bounds.size.y / 2f;
        float camTopY = mainCamera.position.y + Camera.main.orthographicSize;
        float blockHeight = block.GetComponent<SpriteRenderer>().bounds.size.y;

        if ((camTopY - blockTopY) < blockHeight * 3)
        {
            MoveCameraUp(cameraMoveAmount);
            RemoveLowestBlockIfHidden();
        }
    }

    void MoveCameraUp(float amount)
    {
        targetCameraPosition = mainCamera.position + new Vector3(0, amount, 0);
        isMovingUp = true;

        if (crane != null)
        {
            craneTargetY = crane.position.y + amount;
        }
    }

    void RemoveLowestBlockIfHidden()
    {
        if (placedBlocks.Count <= 1) return;

        GameObject lowest = placedBlocks.OrderBy(b => b.transform.position.y).FirstOrDefault();
        if (lowest == null) return;

        SpriteRenderer sr = lowest.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float blockTopY = sr.bounds.max.y;
        float cameraBottomY = mainCamera.position.y - Camera.main.orthographicSize;

        if (blockTopY < cameraBottomY)
        {
            placedBlocks.Remove(lowest);
            originalPositions.Remove(lowest);
            Destroy(lowest);
            Debug.Log("Bloco mais baixo removido.");
        }
    }

    public void TriggerCollapse()
    {
        collapseTriggered = true;
        StartCoroutine(CollapseSequence());
    }

    private IEnumerator CollapseSequence()
    {
        foreach (GameObject block in placedBlocks)
        {
            if (block == null) continue;

            Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = block.AddComponent<Rigidbody2D>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0.5f;

            float torque = Random.Range(-10f, -5f);
            rb.AddTorque(torque, ForceMode2D.Force);

            float sideForce = Random.Range(-0.5f, -0.2f);
            rb.linearVelocity = new Vector2(sideForce, rb.linearVelocity.y);

            yield return new WaitForSeconds(0.05f);
        }
    }

    void Update()
    {
        placedBlocks.RemoveAll(b => b == null || b.transform.position.y < destroyBelowY);

        if (!collapseTriggered)
        {
            shakeTimer += Time.deltaTime * shakeFrequency;

            int total = placedBlocks.Count;

            if (total >= 10)
            {
                int tiers = (total - 10) / 10;
                float shakeIncrease = tiers * 0.05f;
                currentShakeAmplitude = Mathf.Min(baseShakeAmplitude + shakeIncrease, maxShakeAmplitude);

                for (int i = 0; i < total; i++)
                {
                    GameObject block = placedBlocks[i];
                    if (block == null || !originalPositions.ContainsKey(block)) continue;

                    float relativeIndex = (float)(i + 1) / total;
                    float strength = relativeIndex * currentShakeAmplitude;

                    Vector3 basePos = originalPositions[block];
                    float offsetX = Mathf.Sin(shakeTimer) * strength;

                    block.transform.position = new Vector3(basePos.x + offsetX, basePos.y, basePos.z);
                }
            }
        }

        if (isMovingUp)
        {
            mainCamera.position = Vector3.Lerp(mainCamera.position, targetCameraPosition, Time.deltaTime * cameraMoveSpeed);

            if (Vector3.Distance(mainCamera.position, targetCameraPosition) < 0.01f)
            {
                mainCamera.position = targetCameraPosition;
                isMovingUp = false;
            }

            if (crane != null)
            {
                Vector3 currentPos = crane.position;
                Vector3 targetPos = new Vector3(currentPos.x, craneTargetY, currentPos.z);
                Vector3 newCranePos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * cameraMoveSpeed);
                crane.position = newCranePos;

                CraneSpawner spawner = Object.FindFirstObjectByType<CraneSpawner>();
                if (spawner != null)
                {
                    spawner.UpdateStartYSmooth(newCranePos.y);
                }
            }
        }
    }
}
