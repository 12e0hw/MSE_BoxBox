using UnityEngine;
using System.Collections;

public class NPC_ExplainManager : MonoBehaviour
{
    [Header("랜덤 패널")]
    public GameObject[] explainPanels;

    private Coroutine hideCoroutine;


    void Awake()
    {
        HideAllPanels();
    }

    //대화창 띄우기
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }

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
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideDelay(1f));
        }
    }

    private IEnumerator HideDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        HideAllPanels();
    }

    void HideAllPanels()
    {
        foreach (GameObject panel in explainPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }
}
