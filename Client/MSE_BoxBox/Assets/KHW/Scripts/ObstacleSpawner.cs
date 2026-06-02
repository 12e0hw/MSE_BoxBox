using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct WeatherObstacleSetting
    {
        public string weatherName; 
        public GameObject obstaclePrefab; 
    }
    
    [Header("Obstacle Settings")]
    [SerializeField] private List<WeatherObstacleSetting> weatherSettings;
    [SerializeField] private float spawnInterval = 8.0f; 

    [Header("Spawn Area")]
    [SerializeField] private float minX = -6.0f;
    [SerializeField] private float maxX = 6.0f;
    [SerializeField] private float minY = -2.0f;
    [SerializeField] private float maxY = 1.5f;

    private GameObject obstaclePrefab; 

    private void OnEnable()
    {
        WeatherUIController.OnGameWeatherRefreshed += SetWeather;
    }

    private void OnDisable()
    {
        WeatherUIController.OnGameWeatherRefreshed -= SetWeather;
    }

    private void Start()
    {
        obstaclePrefab = null;
        if (!string.IsNullOrWhiteSpace(WeatherUIController.CurrentWeather))
        {
            SetWeather(WeatherUIController.CurrentWeather);
        }

        StartCoroutine(SpawnObstacleRoutine());
    }

    private void HandleWeatherOverride(string weather, string gameEffect)
    {
        SetWeather(weather);
    }

    public void SetWeather(string weatherFromServer)
    {
        if (string.IsNullOrWhiteSpace(weatherFromServer)) return;

        string targetWeather = weatherFromServer.ToUpper().Trim();
        Debug.Log($" {targetWeather}");

        WeatherObstacleSetting matchedSetting = weatherSettings.Find(x => x.weatherName.ToUpper() == targetWeather);

        if (matchedSetting.obstaclePrefab != null)
        {
            obstaclePrefab = matchedSetting.obstaclePrefab;
        }
        else
        {
            obstaclePrefab = null;
        }
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
        if (obstaclePrefab == null) return;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}