using UnityEngine;
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

    private List<GameObject> placedBlocks = new List<GameObject>();
    private bool isMovingUp = false;
    private Vector3 targetCameraPosition;
    private float craneTargetY = 0f;

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
            placedBlocks.Add(block);

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
            Destroy(lowest);
            Debug.Log("Bloco mais baixo removido.");
        }
    }

    void Update()
    {
        placedBlocks.RemoveAll(b => b == null || b.transform.position.y < destroyBelowY);

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