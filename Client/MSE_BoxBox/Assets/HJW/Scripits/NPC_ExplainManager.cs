using UnityEngine;

public class NPC_ExplainManager : MonoBehaviour
{
    [Header("랜덤 패널")]
    public GameObject[] explainPanels;       


    void Awake()
    {
        HideAllPanels();
    }

    //대화창 띄우기
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HideAllPanels();

            if (explainPanels.Length > 0)
            {
                int randomIndex = Random.Range(0, explainPanels.Length);
                explainPanels[randomIndex].SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HideAllPanels();
        }
    }

    void HideAllPanels()
    {
        foreach (GameObject panel in explainPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }
}
