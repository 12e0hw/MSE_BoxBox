using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

public class NameChangeManager : MonoBehaviour
{
    [Header("입력창 UI")]
    public TMP_InputField player1Input; // 1P 닉네임 입력창
    public TMP_InputField player2Input; // 2P 닉네임 입력창

    [Header("결과 패널 UI")]
    public GameObject successPanel;     // NameSuccessPanel 연결
    public GameObject failPanel;        // NameFailPanel 연결

    // CHANGE 버튼의 On Click() 에 연결할 함수입니다.
    public void OnChangeNameButtonClicked()
    {
        // 1. 입력된 텍스트 가져오기 (Trim()을 써서 실수로 입력한 띄어쓰기 공백을 제거합니다)
        string p1Name = player1Input.text.Trim();
        string p2Name = player2Input.text.Trim();

        // 3. 실패 조건 B: 두 플레이어의 닉네임이 서로 똑같을 경우 (중복)
        if (p1Name == p2Name)
        {
            Debug.Log("이름 변경 실패: 1P와 2P의 닉네임이 중복됩니다.");
            ShowFailPanel();
            return;
        }

        // (추가 확장) 만약 기존에 저장된 이름과 똑같이 입력했을 때도 
        // 실패로 처리하고 싶다면 이곳에 조건문을 추가하면 됩니다.

        // 4. 모든 조건을 무사히 통과했을 경우 (성공)
        Debug.Log($"이름 변경 성공! 1P: {p1Name}, 2P: {p2Name}");
        ShowSuccessPanel();
        
        // TODO: 이곳에 이전에 알려드렸던 '서버 모킹(Mocking) 코루틴'이나 
        // 실제 서버 전송 코드를 연결하여 이름 데이터를 저장하면 완벽합니다!
    }

    // 실패 패널을 띄우는 함수
    private void ShowFailPanel()
    {
        successPanel.SetActive(false); // 성공 패널은 끄고
        failPanel.SetActive(true);     // 실패 패널을 켭니다
    }

    // 성공 패널을 띄우는 함수
    private void ShowSuccessPanel()
    {
        failPanel.SetActive(false);    // 실패 패널은 끄고
        successPanel.SetActive(true);  // 성공 패널을 켭니다
    }
}