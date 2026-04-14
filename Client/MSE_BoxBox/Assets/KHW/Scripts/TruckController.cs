using UnityEngine;

public class TruckController : MonoBehaviour
{
    public BoxColor truckColor; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        BoxController incomingBox = other.GetComponent<BoxController>();

        if (incomingBox != null)
        {
            if (incomingBox.boxColor == truckColor)
            {
                Debug.Log($"+{incomingBox.scoreValue}점");
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(incomingBox.scoreValue);
                }
                
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log($"오배송");
                // 필요하다면 여기서 감점 로직 추가
                // GameManager.Instance.AddScore(-1);
            }
        }
    }
}