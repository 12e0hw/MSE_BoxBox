using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; 
using System.Collections.Generic;

public class SettingChangeManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingPanel;
    public GameObject ControlKey1Panel;
    public GameObject ControlKey2Panel;
    public GameObject NameFailPanel;
    public GameObject NameSuccessPanel;
    public GameObject SelectCityPanel; 

    void Start()
    {
        if(ControlKey1Panel != null) ControlKey1Panel.SetActive(false);
        if(ControlKey2Panel != null) ControlKey2Panel.SetActive(false);
        if(NameSuccessPanel != null) NameSuccessPanel.SetActive(false);
        if(NameFailPanel != null) NameFailPanel.SetActive(false);
        if(SelectCityPanel != null) SelectCityPanel.SetActive(false);
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
    public static Action OnCityChanged; 
    private static string sessionCity = "suwon";
    public Transform contentParent;    
    public GameObject cityItemPrefab;  
    public TMP_Text currentCityText;

    private string[] cityList = {
        "Seoul", "Daejeon", "Jeonju", "Busan", "Gwangju",
        "Suwon", "Incheon", "Daegu", "Ulsan", "Jeju",
        "Cheonan", "Cheongju", "Chuncheon", "Gangneung", "Pohang",
        "Changwon", "Gimhae", "Jinju", "Gunsan", "Iksan",
        "Mokpo", "Yeosu", "Suncheon", "Andong", "Gumi",
        "Gyeongju", "Asan", "Seosan", "Dangjin", "Gongju"
    };

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
        foreach (string city in cityList)
        {
            GameObject btnObj = Instantiate(cityItemPrefab, contentParent);
            btnObj.GetComponentInChildren<TMP_Text>().text = city;
            
            string capturedCity = city; 
            btnObj.GetComponent<Button>().onClick.AddListener(() => OnCitySelected(capturedCity));
        }
    }

    public void OnCitySelected(string newCity)
    {
        sessionCity = newCity;
        currentCityText.text = sessionCity;
        SelectCityPanel.SetActive(false);
        //다른 스크립트에 전달 때 이용
        OnCityChanged?.Invoke();
    }

    public static string GetSavedCity()
    {
        return sessionCity;
    }
}
