using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    public Vector2 direction; 
    public float speed = 1.0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;

        }
    }
}