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

        if (box != null && !box.IsDelivered)
        {
            // bool isSuccess = deliveryManager.TryDeliver(box, truckColor);
            
            // if (isSuccess)
            // {
            //     Destroy(other.gameObject);
            // }
        }
    }
}