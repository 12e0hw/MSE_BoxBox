using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSoundBinder : MonoBehaviour
{
    [Header("Button Binding")]
    [SerializeField] private bool includeInactiveButtons = true;

    private Button[] boundButtons;

    private void OnEnable()
    {
        BindButtonSounds();
    }

    private void OnDisable()
    {
        UnbindButtonSounds();
    }

    // Bind button click sound events to child buttons.
    public void BindButtonSounds()
    {
        boundButtons = GetComponentsInChildren<Button>(includeInactiveButtons);

        for (int i = 0; i < boundButtons.Length; i++)
        {
            Button button = boundButtons[i];

            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveListener(PlayButtonClickSound);
            button.onClick.AddListener(PlayButtonClickSound);
        }

        Debug.Log($"[ButtonClickSoundBinder] Button sound binding completed: {boundButtons.Length} buttons");
    }

    // Remove button click sound events from bound buttons.
    private void UnbindButtonSounds()
    {
        if (boundButtons == null)
        {
            return;
        }

        for (int i = 0; i < boundButtons.Length; i++)
        {
            Button button = boundButtons[i];

            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveListener(PlayButtonClickSound);
        }
    }

    // Play the button click sound through the BGM manager.
    private void PlayButtonClickSound()
    {
        if (BGM_Manager.instance == null)
        {
            Debug.LogWarning("[ButtonClickSoundBinder] BGM_Manager.instance is missing.");
            return;
        }

        BGM_Manager.instance.PlayButtonSound();
    }
}