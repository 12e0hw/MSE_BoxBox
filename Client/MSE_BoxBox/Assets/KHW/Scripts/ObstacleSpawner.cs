using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs; 
    [SerializeField] private float spawnInterval = 8.0f; 

    [Header("Spawn Area")]
    [SerializeField] private float minX = -6.0f;
    [SerializeField] private float maxX = 6.0f;
    [SerializeField] private float minY = -2.0f;
    [SerializeField] private float maxY = 1.5f;

    private void Start()
    {
        StartCoroutine(SpawnObstacleRoutine());
    }

    private IEnumerator SpawnObstacleRoutine()
    {
        while (true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        // 프리팹 목록 중 랜덤으로 하나 선택 (테스트)
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}