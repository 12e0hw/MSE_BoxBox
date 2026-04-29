using UnityEngine;

namespace LJC
{
    public class TruckController : MonoBehaviour
    {
        [Header("Truck Data")]
        [SerializeField] private BoxColor truckColor;

        [Header("Reference")]
        [SerializeField] private DeliveryManager deliveryManager;

        public BoxColor TruckColor => truckColor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            BoxController box = other.GetComponent<BoxController>();

            if (box == null)
                return;

            if (deliveryManager == null)
                return;

            bool success = deliveryManager.TryDeliver(box, this);

            if (success)
            {
                Destroy(box.gameObject);
            }
        }
    }
}