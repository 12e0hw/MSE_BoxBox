using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    public Vector2 direction; 
    public float speed = 1.0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsCarriedBigBoxGroup(other))
        {
            return;
        }

        Rigidbody2D rb = other.attachedRigidbody != null ? other.attachedRigidbody : other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Move loose objects along the conveyor direction.
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    private bool IsCarriedBigBoxGroup(Collider2D other)
    {
        // Big boxes held by players should not be pushed by the conveyor.
        Player player = other.GetComponentInParent<Player>();

        if (player != null && player.IsHoldingBigBox)
        {
            return true;
        }

        BigBoxCarryController bigBox = other.GetComponentInParent<BigBoxCarryController>();
        return bigBox != null && bigBox.IsHeld;
    }
}
