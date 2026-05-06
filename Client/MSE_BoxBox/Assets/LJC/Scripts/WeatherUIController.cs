using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherUIController : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string serverBaseUrl = "http://localhost:8080";

    [Header("UI Text")]
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

    [Header("Option")]
    [SerializeField] private bool loadOnStart = true;

    private void Start()
    {
        ApplyWeatherImageSize();

        if (loadOnStart)
        {
            RefreshWeather();
        }
    }

    public void RefreshWeather()
    {
        StartCoroutine(GetGameWeather());
    }

    private IEnumerator GetGameWeather()
    {
        string url = serverBaseUrl + "/api/external/weather/game";

        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowError("날씨 정보를 불러오지 못했습니다.");
            Debug.LogError("Weather API Error: " + request.error);
            yield break;
        }

        WeatherApiResponse response = JsonUtility.FromJson<WeatherApiResponse>(request.downloadHandler.text);

        if (response == null || response.success == false || response.data == null)
        {
            ShowError("날씨 응답 데이터가 올바르지 않습니다.");
            Debug.LogError("Invalid Weather Response: " + request.downloadHandler.text);
            yield break;
        }

        UpdateWeatherUI(response.data);
    }

    private void UpdateWeatherUI(GameWeatherData data)
    {
        string weather = data.weather.ToUpper();
        string gameEffect = data.gameEffect.ToUpper();

        if (weatherText != null)
        {
            weatherText.text = "Weather: " + ConvertWeatherName(weather);
        }

        if (temperatureText != null)
        {
            temperatureText.text = "Temperature: " + data.temperature.ToString("0.0") + "°C";
        }

        if (effectText != null)
        {
            effectText.text = "Effect: " + ConvertGameEffectName(gameEffect);
        }

        if (weatherImage != null)
        {
            weatherImage.sprite = GetWeatherSprite(weather);
            weatherImage.enabled = weatherImage.sprite != null;

            ApplyWeatherImageSize();
        }
    }

    private void ApplyWeatherImageSize()
    {
        if (weatherImage == null)
        {
            return;
        }

        RectTransform imageRectTransform = weatherImage.GetComponent<RectTransform>();

        imageRectTransform.sizeDelta = weatherImageSize;

        weatherImage.preserveAspect = true;
        weatherImage.type = Image.Type.Simple;
    }

    private Sprite GetWeatherSprite(string weather)
    {
        if (weather == "RAIN")
        {
            return rainSprite;
        }

        if (weather == "CLEAR")
        {
            return clearSprite;
        }

        return defaultSprite;
    }

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

        return weather;
    }

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

        return gameEffect;
    }

    private void ShowError(string message)
    {
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
        public string weather;
        public float temperature;
        public string gameEffect;
    }
}