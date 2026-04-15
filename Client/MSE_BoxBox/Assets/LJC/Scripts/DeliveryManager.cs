using System;
using UnityEngine;

namespace LJC
{
    public class DeliveryManager : MonoBehaviour
    {
        public event Action<BoxController, TruckController> OnDeliverySuccess;
        public event Action<BoxController, TruckController> OnDeliveryFail;

        public bool TryDeliver(BoxController box, TruckController truck)
        {
            if (box == null || truck == null)
                return false;

            if (box.IsDelivered)
                return false;

            bool success = box.Color == truck.TruckColor;

            if (success)
            {
                box.MarkDelivered();
                OnDeliverySuccess?.Invoke(box, truck);
            }
            else
            {
                OnDeliveryFail?.Invoke(box, truck);
            }

            return success;
        }
    }
}