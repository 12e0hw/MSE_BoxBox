using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    [Header("Spawn Settings")] public GameObject[] boxPrefabs;
    public Transform spawnPoint;
    public float interval = 5f; // 생성 간격
    
    private Coroutine spawnCoroutine;

    public void SetSpawnInterval(float newInterval)
    {
        interval = Mathf.Max(0.1f, newInterval);
    }

    public void StartSpawning()
    {
        StopSpawning();
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public void ResetSpawner()
    {
        StopSpawning();
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (boxPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, boxPrefabs.Length);
                Instantiate(boxPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
            }
        }
    }
}