using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System; 
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ChangeKey : MonoBehaviour
{
    public static Action OnkeyChanged; //키 설정 변경 캐릭터에게 전달

    // 게임 실행 중일때만 키 임시 저장
    private static Dictionary<string, string> sessionKeys = new Dictionary<string, string>(); 

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
    private Dictionary<string, Key> pendingKeys = new Dictionary<string, Key>();

    //p1 can choose this keys
    private Key[] p1_Keys = {
        Key.Q, Key.W, Key.E, Key.R, Key.T,
        Key.A, Key.S, Key.D, Key.F, Key.G,
        Key.Z, Key.X, Key.C, Key.V, Key.B,
        Key.LeftShift, Key.Escape, Key.Space
    };

    //p2 can choose this keys
    private Key[] p2_Keys = {
        Key.UpArrow, Key.DownArrow, Key.LeftArrow, Key.RightArrow,
        Key.Y, Key.U, Key.I, Key.O, Key.P,
        Key.H, Key.J, Key.K, Key.L,
        Key.N, Key.M, Key.Comma, Key.Period, Key.Slash,
        Key.RightShift, Key.Enter, Key.Backspace
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
    private void GenerateKeyButtons(Key[] keysToGenerate)
    {
    foreach (Key key in keysToGenerate)
        {
            GameObject btnObj = Instantiate(keyItemPrefab, contentParent);
            btnObj.GetComponentInChildren<TMP_Text>().text = GetKeyDisplayName(key);
            btnObj.GetComponent<Button>().onClick.AddListener(() => OnKeySelected(key));
        }
    }

    public void OnKeySelected(Key newKey)
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
            sessionKeys[kvp.Key] = kvp.Value.ToString();
        }
        pendingKeys.Clear(); 
        player1KeyPanel.SetActive(false);
        player2KeyPanel.SetActive(false);
        //설정 저장시 캐릭터 바로 반영
        OnkeyChanged?.Invoke();
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
        p1_upKeyText.text = GetKeyDisplayName(GetSavedKey("P1_UpKey", Key.W));
        p1_downKeyText.text = GetKeyDisplayName(GetSavedKey("P1_DownKey", Key.S));
        p1_leftKeyText.text = GetKeyDisplayName(GetSavedKey("P1_LeftKey", Key.A));
        p1_rightKeyText.text = GetKeyDisplayName(GetSavedKey("P1_RightKey", Key.D));
        p1_interactKeyText.text = GetKeyDisplayName(GetSavedKey("P1_InteractKey", Key.C));
        p1_fireKeyText.text = GetKeyDisplayName(GetSavedKey("P1_FireKey", Key.V));
        p1_runKeyText.text = GetKeyDisplayName(GetSavedKey("P1_RunKey", Key.B));

        p2_upKeyText.text = GetKeyDisplayName(GetSavedKey("P2_UpKey", Key.UpArrow));
        p2_downKeyText.text = GetKeyDisplayName(GetSavedKey("P2_DownKey", Key.DownArrow));
        p2_leftKeyText.text = GetKeyDisplayName(GetSavedKey("P2_LeftKey", Key.LeftArrow));
        p2_rightKeyText.text = GetKeyDisplayName(GetSavedKey("P2_RightKey", Key.RightArrow));
        p2_interactKeyText.text = GetKeyDisplayName(GetSavedKey("P2_InteractKey", Key.I));
        p2_fireKeyText.text = GetKeyDisplayName(GetSavedKey("P2_FireKey", Key.O));
        p2_runKeyText.text = GetKeyDisplayName(GetSavedKey("P2_RunKey", Key.P));
    }

    //PlayerInputHandler 키를 불러갈 수 있도록 제공
    public static Key GetSavedKey(string prefKey, Key defaultKey)
    {
        string savedValue = sessionKeys.ContainsKey(prefKey) ? sessionKeys[prefKey] : defaultKey.ToString();
        
        if (Enum.TryParse(savedValue, out Key parsedKey))
        {
            return parsedKey;
        }
        return defaultKey;
    }

    private string GetKeyDisplayName(Key key)
    {
        switch (key)
        {
            case Key.UpArrow: return "↑";
            case Key.DownArrow: return "↓";
            case Key.LeftArrow: return "←";
            case Key.RightArrow: return "→";
            case Key.LeftShift: return "L-SHIFT";
            case Key.RightShift: return "R-SHIFT";
            case Key.Space: return "SPACE";
            case Key.Escape: return "ESC";
            case Key.Enter: return "ENTER";       
            case Key.Backspace: return "BACKSPACE";
            case Key.Comma: return ",";
            case Key.Period: return ".";
            case Key.Slash: return "/";
            default: return key.ToString();
        }
    }
}
