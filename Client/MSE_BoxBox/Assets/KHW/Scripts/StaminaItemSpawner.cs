using System.Collections;
using UnityEngine;

public class StaminaItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject staminerItem; 
    [SerializeField] private float spawnInterval = 5.0f; 

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
        if (staminerItem == null) return;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);


        Instantiate(staminerItem, spawnPosition, Quaternion.identity);
    }
}