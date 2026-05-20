using UnityEngine;
using LJC;

public class TruckController : MonoBehaviour
{
    [SerializeField] private BoxColor truckColor;
    public BoxColor TruckColor => truckColor;
    private DeliveryManager deliveryManager;

    private void Awake()
    {
        deliveryManager = Object.FindFirstObjectByType<DeliveryManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        BoxController box = other.GetComponent<BoxController>();


        if (box == null)
        {
            box = other.GetComponentInParent<BoxController>();
        }

        if (box == null)
        {
            return;
        }

        if (box.IsDelivered)
        {
            return;
        }

        if (deliveryManager == null)
        {
            Debug.LogError("[TruckController] DeliveryManager를 찾지 못했습니다.");
            return;
        }

        bool isSuccess = deliveryManager.TryDeliver(box, this);

        if (isSuccess)
        {
            Destroy(box.gameObject);
        }
    }
}