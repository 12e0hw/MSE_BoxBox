using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System; 
using System.Collections.Generic; 

public class ChangeKey : MonoBehaviour
{
    [Header("ControlKey Setting")]
    public GameObject selectKeyPanel; 
    public Transform contentParent;    
    public GameObject keyItemPrefab;
    public GameObject player1KeyPanel;
    public GameObject player2KeyPanel;   

    [Header("Player 1 Text")]
    public TMP_Text p1_upKeyText;
    public TMP_Text p1_downKeyText;
    public TMP_Text p1_leftKeyText;
    public TMP_Text p1_rightKeyText;
    public TMP_Text p1_interactKeyText;
    public TMP_Text p1_fireKeyText;
    public TMP_Text p1_runKeyText;

    [Header("Player 2 Text")]
    public TMP_Text p2_upKeyText;
    public TMP_Text p2_downKeyText;
    public TMP_Text p2_leftKeyText;
    public TMP_Text p2_rightKeyText;
    public TMP_Text p2_interactKeyText;
    public TMP_Text p2_fireKeyText;
    public TMP_Text p2_runKeyText;

    private string currentActionName; 
    private Dictionary<string, KeyCode> pendingKeys = new Dictionary<string, KeyCode>();

    //p1 can choose this keys
    private KeyCode[] p1_Keys = {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B,
        KeyCode.LeftShift, KeyCode.Escape, KeyCode.Space
    };

    //p2 can choose this keys
    private KeyCode[] p2_Keys = {
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.N, KeyCode.M, KeyCode.Comma, KeyCode.Period, KeyCode.Slash,
        KeyCode.RightShift, KeyCode.Return, KeyCode.Backspace
    };

    private void Start()
    {
        selectKeyPanel.SetActive(false); 
        RefreshUI(); // 시작할 때 현재 저장된 키값으로 화면 세팅
    }

    // p1/p2 divide
    public void OpenPanel(string actionName)
    {
        currentActionName = actionName;      

        // clear
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // divide a p1_ p2_
        if (actionName.StartsWith("P1_"))
        {
            GenerateKeyButtons(p1_Keys);
        }
        else if (actionName.StartsWith("P2_"))
        {
            GenerateKeyButtons(p2_Keys);
        }

        selectKeyPanel.SetActive(true);      
    }

    // 키들 프리팹 복사하기
    private void GenerateKeyButtons(KeyCode[] keysToGenerate)
    {
    foreach (KeyCode key in keysToGenerate)
        {
            GameObject btnObj = Instantiate(keyItemPrefab, contentParent);
            btnObj.GetComponentInChildren<TMP_Text>().text = GetKeyDisplayName(key);
            btnObj.GetComponent<Button>().onClick.AddListener(() => OnKeySelected(key));
        }
    }

    public void OnKeySelected(KeyCode newKey)
    {
        pendingKeys[currentActionName] = newKey;
        string displayName = GetKeyDisplayName(newKey);

        if (currentActionName == "P1_UpKey") p1_upKeyText.text = displayName;
        else if (currentActionName == "P1_DownKey") p1_downKeyText.text = displayName;
        else if (currentActionName == "P1_LeftKey") p1_leftKeyText.text = displayName;
        else if (currentActionName == "P1_RightKey") p1_rightKeyText.text = displayName;
        else if (currentActionName == "P1_InteractKey") p1_interactKeyText.text = displayName;
        else if (currentActionName == "P1_FireKey") p1_fireKeyText.text = displayName;
        else if (currentActionName == "P1_RunKey") p1_runKeyText.text = displayName;
        
        else if (currentActionName == "P2_UpKey") p2_upKeyText.text = displayName;
        else if (currentActionName == "P2_DownKey") p2_downKeyText.text = displayName;
        else if (currentActionName == "P2_LeftKey") p2_leftKeyText.text = displayName;
        else if (currentActionName == "P2_RightKey") p2_rightKeyText.text = displayName;
        else if (currentActionName == "P2_InteractKey") p2_interactKeyText.text = displayName;
        else if (currentActionName == "P2_FireKey") p2_fireKeyText.text = displayName;
        else if (currentActionName == "P2_RunKey") p2_runKeyText.text = displayName;

        selectKeyPanel.SetActive(false);
    }

    public void SaveSettings()
    {
        foreach (var kvp in pendingKeys)
        {
            PlayerPrefs.SetString(kvp.Key, kvp.Value.ToString());
        }
        PlayerPrefs.Save(); 
        pendingKeys.Clear(); 
        player1KeyPanel.SetActive(false);
        player2KeyPanel.SetActive(false);
    }

    public void CancelSettings()
    {
        pendingKeys.Clear();
        RefreshUI();
        player1KeyPanel.SetActive(false);
        player2KeyPanel.SetActive(false);
    }

    //기본 설정 키
    private void RefreshUI()
    {
        p1_upKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_UpKey", "W")));
        p1_downKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_DownKey", "S")));
        p1_leftKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_LeftKey", "A")));
        p1_rightKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_RightKey", "D")));
        p1_interactKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_InteractKey", "V")));
        p1_fireKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_FireKey", "B")));
        p1_runKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P1_RunKey", "N")));

        p2_upKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_UpKey", "UpArrow")));
        p2_downKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_DownKey", "DownArrow")));
        p2_leftKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_LeftKey", "LeftArrow")));
        p2_rightKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_RightKey", "RightArrow")));
        p2_interactKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_InteractKey", "I")));
        p2_fireKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_FireKey", "O")));
        p2_runKeyText.text = GetKeyDisplayName((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("P2_RunKey", "P")));
    }

    private string GetKeyDisplayName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.UpArrow: return "↑";
            case KeyCode.DownArrow: return "↓";
            case KeyCode.LeftArrow: return "←";
            case KeyCode.RightArrow: return "→";
            case KeyCode.LeftShift: return "L-SHIFT";
            case KeyCode.RightShift: return "R-SHIFT";
            case KeyCode.Space: return "SPACE";
            case KeyCode.Escape: return "ESC";
            case KeyCode.Return: return "ENTER";       
            case KeyCode.Backspace: return "BACKSPACE";
            case KeyCode.Comma: return ",";
            case KeyCode.Period: return ".";
            case KeyCode.Slash: return "/";
            default: return key.ToString();
        }
    }
}
