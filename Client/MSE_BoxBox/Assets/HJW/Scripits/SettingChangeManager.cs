using UnityEngine;
using UnityEngine.UI;

public class SettingChangeManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingPanel;
    public GameObject ControlKey1Panel;
    public GameObject ControlKey2Panel;
    public GameObject NameFailPanel;
    public GameObject NameSuccessPanel;

    void Start()
    {
        if(ControlKey1Panel != null) ControlKey1Panel.SetActive(false);
        if(ControlKey2Panel != null) ControlKey2Panel.SetActive(false);
        if(NameSuccessPanel != null) NameSuccessPanel.SetActive(false);
        if(NameFailPanel != null) NameFailPanel.SetActive(false);
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
}
