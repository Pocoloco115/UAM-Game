using UnityEngine;
using System.Collections.Generic;

using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private GameObject spikePlatformPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float verticalDistance = 3f;
    [SerializeField] private float minHorizontalDistance = 1.2f; 
    [SerializeField] private float maxHorizontalDistance = 4f;   
    [SerializeField] private float maxVerticalStep = 4.5f;       

    private float nextSpawnY;
    private float minX;
    private float maxX;

    private Vector2 lastPlatformPos;

    void Start()
    {
        CalculateCameraBounds();

        nextSpawnY = player.position.y + 2f;
        lastPlatformPos = player.position;

        for (int i = 0; i < 5; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (cameraTransform.position.y + 10f > nextSpawnY)
        {
            SpawnPlatform();
        }
    }

    void CalculateCameraBounds()
    {
        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        minX = -halfWidth + 2f;
        maxX = halfWidth - 2f;
    }

    void SpawnPlatform()
    {
        Vector3 spawnPos;

        int guard = 0; 

        do
        {
            float x = Random.Range(minX, maxX);
            float y = nextSpawnY;

            spawnPos = new Vector3(x, y, 0);

            guard++;
            if (guard > 35)
            {
                break;
            }

        }
        while (!IsPositionReachable(spawnPos));

        GameObject prefab = ChoosePlatformPrefab(spawnPos);

        Instantiate(prefab, spawnPos, Quaternion.identity);

        lastPlatformPos = spawnPos;
        nextSpawnY += verticalDistance;
    }

    bool IsPositionReachable(Vector3 newPos)
    {
        float dx = Mathf.Abs(newPos.x - lastPlatformPos.x);
        float dy = newPos.y - lastPlatformPos.y;

        if (dx < minHorizontalDistance || dx > maxHorizontalDistance)
            return false;

        if (dy > maxVerticalStep)
            return false;

        return true;
    }

    GameObject ChoosePlatformPrefab(Vector3 spawnPosition)
    {
        bool nearEdge = Mathf.Abs(spawnPosition.x) > Mathf.Abs(maxX) * 0.7f;
        bool riskyJump = spawnPosition.y - lastPlatformPos.y > maxVerticalStep * 0.8f;

        if (nearEdge || riskyJump)
        {
            return platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        }
        else
        {
            if (Random.value < 0.2f)
                return spikePlatformPrefab;

            return platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        }
    }
}


