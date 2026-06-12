using UnityEngine;
using System.Collections;

public class NPC_ExplainManager : MonoBehaviour
{
    [Header("Random Panel")]
    public GameObject[] explainPanels;

    private Coroutine hideCoroutine;

    //turned off when the scene first loads
    void Awake()
    {
        HideAllPanels();
    }

    //turn on explain panel when collide with this NPC and player.
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

    //turn off explain panel when collides end
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

    // Coroutine that waits for the specified delay time, then hides all panels
    private IEnumerator HideDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        HideAllPanels();
    }

    // Helper method to iterate through all registered panels and deactivate them
    void HideAllPanels()
    {
        foreach (GameObject panel in explainPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }
}
