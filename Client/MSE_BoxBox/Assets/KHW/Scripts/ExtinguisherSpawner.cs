using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherSpawner : MonoBehaviour
{
    [Header("Extinguisher Settings")]
    [SerializeField] private GameObject extinguisherPrefab; 
    [SerializeField] private float spawnInterval = 4.0f; 

    [Header("Spawn Area")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private List<GameObject> spawnedExtinguishers = new List<GameObject>();
    private bool isClearWeather = false;

    private void OnEnable()
    {
        WeatherUIController.OnGameWeatherRefreshed += SetWeather;
        SettingChangeManager.OnWeatherOverrideRequested += HandleWeatherOverride;

    }

    private void OnDisable()
    {
        WeatherUIController.OnGameWeatherRefreshed -= SetWeather;
        SettingChangeManager.OnWeatherOverrideRequested -= HandleWeatherOverride;
    }

    private void Start()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            spawnedExtinguishers.Add(null);
        }

        if (!string.IsNullOrWhiteSpace(WeatherUIController.CurrentWeather))
        {
            SetWeather(WeatherUIController.CurrentWeather);
        }

        SpawnExtinguisher(isInitialSpawn: true);
        StartCoroutine(SpawnExtinguisherRoutine());
    }

    private void HandleWeatherOverride(string weather, string gameEffect)
    {
        SetWeather(weather);
    }

    public void SetWeather(string weatherFromServer)
    {
        if (string.IsNullOrWhiteSpace(weatherFromServer)) return;

        isClearWeather = (weatherFromServer.ToUpper().Trim() == "CLEAR");
    }

    private IEnumerator SpawnExtinguisherRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnExtinguisher(isInitialSpawn: false);
        }
    }

    private void SpawnExtinguisher(bool isInitialSpawn)
    {
        if (!isClearWeather) return;
        if (extinguisherPrefab == null) return;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnedExtinguishers[i] == null)
            {
                Transform targetPoint = spawnPoints[i];
                GameObject newExtinguisher = Instantiate(extinguisherPrefab, targetPoint.position, Quaternion.identity);
                
                spawnedExtinguishers[i] = newExtinguisher;

                if (isInitialSpawn == false)
                {
                    return;
                }
            }
        }
    }
}