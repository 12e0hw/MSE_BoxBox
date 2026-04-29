using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] private BoxColor truckColor;

    public BoxColor TruckColor => truckColor;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        BoxController incomingBox = other.GetComponent<BoxController>();

        if (incomingBox != null)
        {
            // DeliveryManager 필요
            // DeliveryManager.Instance.CheckDelivery(incomingBox, this);
            
            // 박스 파괴는 여기서 해도 되고, 매니저가 해도 ㅇㅋㅇㅋ
            Destroy(other.gameObject);
        }
    }
}