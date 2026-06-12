using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SettingChangeManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingPanel;
    public GameObject ControlKey1Panel;
    public GameObject ControlKey2Panel;
    public GameObject NameFailPanel;
    public GameObject NameSuccessPanel;
    public GameObject SelectCityPanel;
    
    //Managing Sound in VolumeManager

    private void Awake()
    {
        // Load the previously selected city from local storage, defaulting to Suwon if none exists
        sessionCity = PlayerPrefs.GetString("SelectedCityName", "Suwon");
        sessionCityId = PlayerPrefs.GetString("SelectedCityId", "SUWON");
    }
    
    void Start()
    {
        if(ControlKey1Panel != null) ControlKey1Panel.SetActive(false);
        if(ControlKey2Panel != null) ControlKey2Panel.SetActive(false);
        if(NameSuccessPanel != null) NameSuccessPanel.SetActive(false);
        if(NameFailPanel != null) NameFailPanel.SetActive(false);
        if(SelectCityPanel != null) SelectCityPanel.SetActive(false);

        ApplyCurrentCityUI();
    }

    // Updates the main settings menu text to reflect the currently selected city.
    private void ApplyCurrentCityUI()
    {
        if (currentCityText != null)
        {
            currentCityText.text = sessionCity;
        }
    }
    
    public void Back()
    {
        if(SettingPanel != null) SettingPanel.SetActive(false);
        if(ControlKey1Panel != null) ControlKey1Panel.SetActive(false);
        if(ControlKey2Panel != null) ControlKey2Panel.SetActive(false);
    }

    public void BacktoSetting()
    {
        if(ControlKey1Panel != null) ControlKey1Panel.SetActive(false);
        if(ControlKey2Panel != null) ControlKey2Panel.SetActive(false);
        if(NameSuccessPanel != null) NameSuccessPanel.SetActive(false);
        if(NameFailPanel != null) NameFailPanel.SetActive(false);
    }

    public void ControlKey1()
    {
        ControlKey1Panel.SetActive(true);
    }

    public void ControlKey2()
    {
        ControlKey2Panel.SetActive(true);
    }

    
    [Header("Change city Name")]
    public Transform contentParent;    
    public GameObject cityItemPrefab;  
    public TMP_Text currentCityText;
    
    [SerializeField] private List<CityOption> cityOptions = new List<CityOption>();
    public static Action<string> OnCityIdChanged; 
    
    // The display name shown to the user in the UI
    private static string sessionCity = "Suwon";
    // The exact ID string required by the weather API
    private static string sessionCityId = "SUWON";

    public void OpenCityPanel()
    {
        // Clear any existing buttons first
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        GenerateCityButtons();
        SelectCityPanel.SetActive(true);      
    }

    private void GenerateCityButtons()
    {
        foreach (CityOption cityOption in cityOptions)
        {
            GameObject buttonObject = Instantiate(cityItemPrefab, contentParent);

            CityItemUI cityItemUI = buttonObject.GetComponent<CityItemUI>();
            if (cityItemUI != null)
            {
                cityItemUI.SetCity(cityOption.cityName, cityOption.flagSprite);
            }

            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                CityOption selectedCityOption = cityOption;
                button.onClick.AddListener(() => OnCitySelected(selectedCityOption));
            }
        }
        
        if (showWeatherTestButtons)
        {
            CreateWeatherTestButton("Clear Test", clearTestSprite, "CLEAR", "NORMAL");
            CreateWeatherTestButton("Rain Test", rainTestSprite, "RAIN", "SLIPPERY_FLOOR");
        }
    }

    public void OnCitySelected(CityOption selectedCity)
    {
        if (selectedCity == null)
        {
            Debug.LogError("Selected city is null.");
            return;
        }

        sessionCity = selectedCity.cityName;
        sessionCityId = selectedCity.cityId;

        PlayerPrefs.SetString("SelectedCityName", sessionCity);
        PlayerPrefs.SetString("SelectedCityId", sessionCityId);
        PlayerPrefs.Save();

        ApplyCurrentCityUI();

        if (SelectCityPanel != null)
        {
            SelectCityPanel.SetActive(false);
        }
        
        Debug.Log("Selected City: " + sessionCity);
        Debug.Log("Selected CityId: " + sessionCityId);
        // Notify other systems
        OnCityIdChanged?.Invoke(sessionCityId);
    }
    
    public void CloseCityPanel()
    {
        if (SelectCityPanel != null)
        {
            SelectCityPanel.SetActive(false);
        }
    }

    public static string GetSavedCity()
    {
        return sessionCity;
    }
    
    public static string GetSavedCityId()
    {
        return sessionCityId;
    }
    
    // Event broadcasted to force a specific weather state
    public static Action<string, string> OnWeatherOverrideRequested;
    
    [Header("Weather Test Buttons")]
    [SerializeField] private bool showWeatherTestButtons = true;
    [SerializeField] private Sprite clearTestSprite;
    [SerializeField] private Sprite rainTestSprite;
    
    private void CreateWeatherTestButton(string buttonText, Sprite iconSprite, string weather, string gameEffect)
    {
        GameObject buttonObject = Instantiate(cityItemPrefab, contentParent);

        CityItemUI cityItemUI = buttonObject.GetComponent<CityItemUI>();
        if (cityItemUI != null)
        {
            cityItemUI.SetCity(buttonText, iconSprite);
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnWeatherTestSelected(weather, gameEffect));
        }
    }
    
    private void OnWeatherTestSelected(string weather, string gameEffect)
    {
        if (SelectCityPanel != null)
        {
            SelectCityPanel.SetActive(false);
        }

        Debug.Log("Weather Test Selected: " + weather + " / " + gameEffect);

        OnWeatherOverrideRequested?.Invoke(weather, gameEffect);
    }

}

[System.Serializable]
public class CityOption
{
    public string cityName;
    public string cityId;
    public Sprite flagSprite;
}

