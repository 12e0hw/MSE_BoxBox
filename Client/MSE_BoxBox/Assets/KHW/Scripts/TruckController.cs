using UnityEngine;
using LJC;

public class TruckController : MonoBehaviour
{
    [SerializeField] private BoxColor truckColor;
    public BoxColor TruckColor => ResolveTruckColor();
    private DeliveryManager deliveryManager;

    private void Awake()
    {
        EnsureDeliveryManager();
        ConfigureDeliveryPhysics();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDeliver(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDeliver(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDeliver(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDeliver(collision.collider);
    }

    private void TryDeliver(Collider2D other)
    {
        BoxController box = other.GetComponent<BoxController>();

        if (box == null)
        {
            box = other.GetComponentInParent<BoxController>();
        }

        if (box != null)
        {
            if (TryDeliverBox(box))
            {
                Destroy(box.gameObject);
            }

            return;
        }

        Player player = other.GetComponentInParent<Player>();

        if (player == null)
        {
            return;
        }

        BoxController carriedBox = player.CurrentCarriedBoxController;

        if (carriedBox == null || !carriedBox.IsSmall)
        {
            return;
        }

        if (TryDeliverBox(carriedBox))
        {
            GameObject deliveredObject = player.CurrentCarriedObject != null
                ? player.CurrentCarriedObject
                : carriedBox.gameObject;

            player.ClearDeliveredCarriedObject(deliveredObject);
            Destroy(deliveredObject);
        }
    }

    private bool TryDeliverBox(BoxController box)
    {
        if (box == null)
        {
            return false;
        }

        if (box.IsDelivered)
        {
            return false;
        }

        if (deliveryManager == null)
        {
            EnsureDeliveryManager();
        }

        if (deliveryManager == null)
        {
            return false;
        }

        return deliveryManager.TryDeliver(box, this);
    }

    private void EnsureDeliveryManager()
    {
        if (deliveryManager == null)
        {
            deliveryManager = Object.FindFirstObjectByType<DeliveryManager>();
        }
    }

    private void ConfigureDeliveryPhysics()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D truckCollider in colliders)
        {
            if (truckCollider != null)
            {
                truckCollider.isTrigger = true;
            }
        }
    }

    private BoxColor ResolveTruckColor()
    {
        if (TryResolveTruckColorFromName(name, out BoxColor resolvedColor))
        {
            return resolvedColor;
        }

        return truckColor;
    }

    private bool TryResolveTruckColorFromName(string targetName, out BoxColor resolvedColor)
    {
        resolvedColor = truckColor;

        if (string.IsNullOrEmpty(targetName))
        {
            return false;
        }

        string normalizedName = targetName.ToLowerInvariant();

        if (normalizedName.Contains("red"))
        {
            resolvedColor = BoxColor.Red;
            return true;
        }

        if (normalizedName.Contains("blue"))
        {
            resolvedColor = BoxColor.Blue;
            return true;
        }

        if (normalizedName.Contains("green"))
        {
            resolvedColor = BoxColor.Green;
            return true;
        }

        return false;
    }
}
