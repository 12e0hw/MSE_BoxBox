using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System; 
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ChangeKey : MonoBehaviour
{
    public static Action OnkeyChanged;

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

    // Allowed keys for Player 1
    private Key[] p1_Keys = {
        Key.Q, Key.W, Key.E, Key.R, Key.T,
        Key.A, Key.S, Key.D, Key.F, Key.G,
        Key.Z, Key.X, Key.C, Key.V, Key.B,
        Key.LeftShift, Key.Space
    };

    // Allowed keys for Player 2
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
        RefreshUI(); // Initialize the UI with the currently saved keys when the script starts
    }

    public void OpenPanel(string actionName)
    {
        currentActionName = actionName;      

        // Clear previously generated buttons to prevent duplicates
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Generate key selection buttons dynamically based on the prefix (P1 or P2)
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

    // Instantiates button prefabs for the provided array of available keys.
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
        // Store the selection in pendingKeys so it doesn't apply immediately until saved
        pendingKeys[currentActionName] = newKey;
        string displayName = GetKeyDisplayName(newKey);

        // Update the specific UI Text element to reflect the newly chosen key
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
        // Transfer all pending changes to the actual session dictionary
        foreach (var kvp in pendingKeys)
        {
            sessionKeys[kvp.Key] = kvp.Value.ToString();
        }
        pendingKeys.Clear(); 
        player1KeyPanel.SetActive(false);
        player2KeyPanel.SetActive(false);

        OnkeyChanged?.Invoke();
    }

    public void CancelSettings()
    {
        pendingKeys.Clear();
        RefreshUI(); // Revert text to match sessionKeys
        player1KeyPanel.SetActive(false);
        player2KeyPanel.SetActive(false);
    }

 
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

    //Retrieves the saved key for a specific action. Used by external scripts like PlayerInputHandler.
    public static Key GetSavedKey(string prefKey, Key defaultKey)
    {
        string savedValue = sessionKeys.ContainsKey(prefKey) ? sessionKeys[prefKey] : defaultKey.ToString();
        
        if (Enum.TryParse(savedValue, out Key parsedKey))
        {
            return parsedKey;
        }
        return defaultKey;
    }

    //Converts a Key enum into a user-friendly
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
            case Key.Enter: return "ENTER";       
            case Key.Backspace: return "BACKSPACE";
            case Key.Comma: return ",";
            case Key.Period: return ".";
            case Key.Slash: return "/";
            default: return key.ToString(); // Defaults to the standard enum name (e.g., "A", "B", "C")
        }
    }
}
