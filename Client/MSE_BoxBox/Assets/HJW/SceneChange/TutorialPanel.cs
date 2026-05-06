using UnityEngine;

public class TutorialNPC : MonoBehaviour
{
    public GameObject startTutorialPanel; 

    public GameObject speechBubble; 

    void Start()
    {
        if (speechBubble != null) speechBubble.SetActive(false);
        
        if (startTutorialPanel != null)
        {
            startTutorialPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void CloseTutorial()
    {
        if (startTutorialPanel != null)
        {
            startTutorialPanel.SetActive(false);
            Time.timeScale = 1f; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speechBubble.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speechBubble.SetActive(false);
        }
    }
}