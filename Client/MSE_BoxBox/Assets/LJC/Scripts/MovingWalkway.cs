using UnityEngine;

public class MovingWalkway : MonoBehaviour
{
    public enum WalkwayDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Movement Settings")]
    [SerializeField] private WalkwayDirection direction = WalkwayDirection.Right;
    [SerializeField] private float moveSpeed = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyMove(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyMove(other);
    }

    // Clear the player's external velocity when leaving the walkway.
    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null || player.IsHoldingBigBox)
        {
            return;
        }

        player.ClearExternalVelocity();
    }

    // Apply walkway movement to the player.
    private void ApplyMove(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null || player.IsHoldingBigBox)
        {
            return;
        }

        player.SetExternalVelocity(GetDirectionVector() * moveSpeed);
    }

    // Convert the selected walkway direction into a Vector2 value.
    private Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case WalkwayDirection.Up:
                return Vector2.up;

            case WalkwayDirection.Down:
                return Vector2.down;

            case WalkwayDirection.Left:
                return Vector2.left;

            case WalkwayDirection.Right:
                return Vector2.right;

            default:
                return Vector2.zero;
        }
    }
}
