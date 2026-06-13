using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherUIController : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string serverBaseUrl = "http://localhost:8080";


    [Header("UI Text")]
    [SerializeField] private TMP_Text cityText;
    [SerializeField] private TMP_Text weatherText;
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private TMP_Text effectText;

    [Header("UI Image")]
    [SerializeField] private Image weatherImage;
    [SerializeField] private Sprite clearSprite;
    [SerializeField] private Sprite rainSprite;
    [SerializeField] private Sprite defaultSprite;

    [Header("Weather Image Size")]
    [SerializeField] private Vector2 weatherImageSize = new Vector2(100f, 100f);
    
    [Header("Country Flag")]
    [SerializeField] private Image countryFlagImage;
    [SerializeField] private List<CityFlagOption> cityFlagOptions = new List<CityFlagOption>();

    private string lastRequestedCityId = "SUWON";
    
    [Header("Option")]
    [SerializeField] private bool loadOnStart = true;

    private Coroutine weatherCoroutine;
    public static Action<string> OnGameWeatherRefreshed;
    public static string CurrentWeather { get; private set; } = "";

    private void OnEnable()
    {
        SettingChangeManager.OnCityIdChanged += RefreshWeatherByCityId;
        SettingChangeManager.OnWeatherOverrideRequested += ApplyWeatherOverride;
    }

    private void OnDisable()
    {
        SettingChangeManager.OnCityIdChanged -= RefreshWeatherByCityId;
        SettingChangeManager.OnWeatherOverrideRequested -= ApplyWeatherOverride;
    }
    
    private void Start()
    {
        ApplyWeatherImageSize();

        if (loadOnStart)
        {
            RefreshWeather();
        }
    }

    // Refresh weather data using the saved city ID.
    public void RefreshWeather()
    {
        string cityId = SettingChangeManager.GetSavedCityId();
        RefreshWeatherByCityId(cityId);
    }
    
    // Start a new weather request for the selected city.
    private void RefreshWeatherByCityId(string cityId)
    {
        // Use Suwon as the default city if no city ID is saved.
        if (string.IsNullOrWhiteSpace(cityId))
        {
            cityId = "SUWON";
        }

        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
        }

        weatherCoroutine = StartCoroutine(GetGameWeather(cityId));
    }

    // Get weather data from the server.
    private IEnumerator GetGameWeather(string cityId)
    {
        if (string.IsNullOrWhiteSpace(cityId))
        {
            cityId = "SUWON";
        }
        
        lastRequestedCityId = cityId;
        
        string url = serverBaseUrl
                     + "/api/external/weather/game?cityId="
                     + UnityWebRequest.EscapeURL(cityId);

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowError("Failed to load weather information.");
            Debug.LogError("Weather API Error: " + request.error);
            Debug.LogError("Weather API Response: " + request.downloadHandler.text);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Weather API Response: " + json);

        WeatherApiResponse response = JsonUtility.FromJson<WeatherApiResponse>(json);

        if (response == null || response.success == false || response.data == null)
        {
            ShowError("Invalid weather response data.");
            Debug.LogError("Invalid Weather Response: " + json);
            yield break;
        }

        UpdateWeatherUI(response.data);
    }

    // Update the weather UI with the received weather data.
    private void UpdateWeatherUI(GameWeatherData data)
    {
        string cityId = data.cityId;

        if (string.IsNullOrWhiteSpace(cityId))
        {
            cityId = lastRequestedCityId;
        }

        string cityName = data.cityName;

        if (string.IsNullOrWhiteSpace(cityName))
        {
            cityName = GetCityNameByCityId(cityId);
        }

        string weather = "";

        if (!string.IsNullOrWhiteSpace(data.weather))
        {
            weather = data.weather.ToUpper();
        }

        string gameEffect = "";

        if (!string.IsNullOrWhiteSpace(data.gameEffect))
        {
            gameEffect = data.gameEffect.ToUpper();
        }

        if (cityText != null)
        {
            cityText.text = cityName;
        }

        if (weatherText != null)
        {
            weatherText.text = ConvertWeatherName(weather);
        }

        if (temperatureText != null)
        {
            temperatureText.text = data.temperature.ToString("0.0") + "\u00B0C";
        }

        if (effectText != null)
        {
            effectText.text = ConvertGameEffectName(gameEffect);
        }

        if (weatherImage != null)
        {
            weatherImage.sprite = GetWeatherSprite(weather);
            weatherImage.enabled = weatherImage.sprite != null;
            ApplyWeatherImageSize();
        }

        UpdateCountryFlag(cityId);

        if (!string.IsNullOrWhiteSpace(weather))
        {
            string formattedWeather = weather.ToUpper().Trim();
        
            CurrentWeather = formattedWeather;
            OnGameWeatherRefreshed?.Invoke(formattedWeather);
        }
    }

    // Apply the configured size to the weather image.
    private void ApplyWeatherImageSize()
    {
        if (weatherImage == null)
        {
            return;
        }

        RectTransform imageRectTransform = weatherImage.GetComponent<RectTransform>();

        if (imageRectTransform != null)
        {
            imageRectTransform.sizeDelta = weatherImageSize;
        }

        weatherImage.preserveAspect = true;
        weatherImage.type = Image.Type.Simple;
    }

    // Return the sprite that matches the weather type.
    private Sprite GetWeatherSprite(string weather)
    {
        if (weather == "RAIN")
        {
            return rainSprite != null ? rainSprite : defaultSprite;
        }

        if (weather == "CLEAR")
        {
            return clearSprite != null ? clearSprite : defaultSprite;
        }

        return defaultSprite;
    }

    // Convert the weather value into display text.
    private string ConvertWeatherName(string weather)
    {
        if (weather == "RAIN")
        {
            return "Rain";
        }

        if (weather == "CLEAR")
        {
            return "Clear";
        }
        
        if (string.IsNullOrWhiteSpace(weather))
        {
            return "Unknown";
        }

        return weather;
    }

    // Convert the game effect value into display text.
    private string ConvertGameEffectName(string gameEffect)
    {
        if (gameEffect == "SLIPPERY_FLOOR")
        {
            return "Slippery Floor";
        }

        if (gameEffect == "NORMAL")
        {
            return "Normal";
        }
        
        if (string.IsNullOrWhiteSpace(gameEffect))
        {
            return "Unknown";
        }

        return gameEffect;
    }

    // Show an error message on the weather UI.
    private void ShowError(string message)
    {
        if (cityText != null)
        {
            cityText.text = "City: " + SettingChangeManager.GetSavedCity();
        }
        
        if (weatherText != null)
        {
            weatherText.text = message;
        }

        if (temperatureText != null)
        {
            temperatureText.text = "";
        }

        if (effectText != null)
        {
            effectText.text = "";
        }

        if (weatherImage != null)
        {
            weatherImage.sprite = defaultSprite;
            weatherImage.enabled = defaultSprite != null;

            ApplyWeatherImageSize();
        }
    }
    
    // Find the city name that matches the city ID.
    private string GetCityNameByCityId(string cityId)
    {
        foreach (CityFlagOption option in cityFlagOptions)
        {
            if (string.Equals(option.cityId, cityId, StringComparison.OrdinalIgnoreCase))
            {
                return option.cityName;
            }
        }

        return SettingChangeManager.GetSavedCity();
    }
    
    // Update the country flag for the selected city.
    private void UpdateCountryFlag(string cityId)
    {
        if (countryFlagImage == null)
        {
            return;
        }

        Sprite flagSprite = null;

        foreach (CityFlagOption option in cityFlagOptions)
        {
            if (string.Equals(option.cityId, cityId, StringComparison.OrdinalIgnoreCase))
            {
                flagSprite = option.flagSprite;
                break;
            }
        }

        countryFlagImage.sprite = flagSprite;
        countryFlagImage.enabled = flagSprite != null;
        countryFlagImage.preserveAspect = true;
    }
    
    // Apply test weather without requesting data from the server.
    private void ApplyWeatherOverride(string weather, string gameEffect)
    {
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }

        string cityId = SettingChangeManager.GetSavedCityId();
        string cityName = SettingChangeManager.GetSavedCity();

        if (cityText != null)
        {
            cityText.text = cityName;
        }

        string normalizedWeather = string.IsNullOrWhiteSpace(weather)
            ? "CLEAR"
            : weather.ToUpper();

        string normalizedGameEffect = string.IsNullOrWhiteSpace(gameEffect)
            ? "NORMAL"
            : gameEffect.ToUpper();

        if (weatherText != null)
        {
            weatherText.text = ConvertWeatherName(normalizedWeather) + " Test";
        }

        if (temperatureText != null)
        {
            temperatureText.text = "Test Mode";
        }

        if (effectText != null)
        {
            effectText.text = ConvertGameEffectName(normalizedGameEffect);
        }

        if (weatherImage != null)
        {
            weatherImage.sprite = GetWeatherSprite(normalizedWeather);
            weatherImage.enabled = weatherImage.sprite != null;
            ApplyWeatherImageSize();
        }

        UpdateCountryFlag(cityId);

        CurrentWeather = normalizedWeather;
        OnGameWeatherRefreshed?.Invoke(normalizedWeather);

        Debug.Log("Weather Override Applied: " + normalizedWeather + " / " + normalizedGameEffect);
    }

    [Serializable]
    public class WeatherApiResponse
    {
        public bool success;
        public string message;
        public GameWeatherData data;
    }

    [Serializable]
    public class GameWeatherData
    {
        public string cityId;
        public string cityName;
        public string weather;
        public float temperature;
        public string gameEffect;
    }
    
    [Serializable]
    public class CityFlagOption
    {
        public string cityId;
        public string cityName;
        public Sprite flagSprite;
    }
}