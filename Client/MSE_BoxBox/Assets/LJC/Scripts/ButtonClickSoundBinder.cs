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

        Debug.Log($"[ButtonClickSoundBinder] 버튼 사운드 연결 완료: {boundButtons.Length}개");
    }

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

    private void PlayButtonClickSound()
    {
        if (BGM_Manager.instance == null)
        {
            Debug.LogWarning("[ButtonClickSoundBinder] BGM_Manager.instance가 없습니다.");
            return;
        }

        BGM_Manager.instance.PlayButtonSound();
    }
}