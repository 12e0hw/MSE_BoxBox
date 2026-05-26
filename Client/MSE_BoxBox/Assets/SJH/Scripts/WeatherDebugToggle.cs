using UnityEngine;

public class WeatherDebugToggle : MonoBehaviour
{
    [SerializeField] private bool isRainy;
    [SerializeField] private bool applyOnStart;

    void Start()
    {
        if (applyOnStart)
        {
            ApplyWeatherState();
        }
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        ApplyWeatherState();
    }

    void ApplyWeatherState()
    {
        string weather = isRainy ? "RAIN" : "CLEAR";
        string gameEffect = isRainy ? "SLIPPERY_FLOOR" : "NORMAL";
        WeatherUIController.SetWeatherState(weather, gameEffect);
    }
}
