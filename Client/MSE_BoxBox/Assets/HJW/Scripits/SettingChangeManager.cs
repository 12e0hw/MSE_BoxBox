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

    private void Awake()
    {
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

    
    [Header("도시 이름 바꾸기")]
    public Transform contentParent;    
    public GameObject cityItemPrefab;  
    public TMP_Text currentCityText;
    
    [SerializeField] private List<CityOption> cityOptions = new List<CityOption>();
    public static Action<string> OnCityIdChanged; 
    
    // 유저에게 보여주기용
    private static string sessionCity = "Suwon";
    // API 호출용
    private static string sessionCityId = "SUWON";

    public void OpenCityPanel()
    {
        // 창을 열 때 기존 버튼 클리어
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
        //다른 스크립트에 전달 때 이용
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
    
    // 강제 날씨 바꾸는 이벤트
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

