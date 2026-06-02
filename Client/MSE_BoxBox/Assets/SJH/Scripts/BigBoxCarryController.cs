using UnityEngine;

public enum BigBoxHoldSide
{
    Left,
    Right
}

[RequireComponent(typeof(BoxController))]
public class BigBoxCarryController : MonoBehaviour
{
    [SerializeField] private float minHorizontalSeparation = 0.15f;
    [SerializeField] private bool requireHorizontalDominance = true;
    [SerializeField] private float movementCastPadding = 0.03f;
    [SerializeField] private LayerMask blockingLayers;

    private BoxController box;
    private Rigidbody2D rb;
    private Player leftHolder;
    private Player rightHolder;
    private Vector2 pendingVelocity;
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[32];

    public bool IsReadyToMove => leftHolder != null && rightHolder != null;
    public Vector2 CurrentVelocity => pendingVelocity;

    void Awake()
    {
        FindReferences();
    }

    void FixedUpdate()
    {
        FindReferences();

        if (!IsReadyToMove)
        {
            pendingVelocity = Vector2.zero;
        }
        else
        {
            pendingVelocity = GetAllowedVelocity(pendingVelocity);
        }

        if (rb != null)
        {
            rb.linearVelocity = pendingVelocity;
        }

        ApplyHolderVelocity(pendingVelocity);
    }

    void OnDisable()
    {
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);
    }

    void OnDestroy()
    {
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);
    }

    public bool TryAttach(Player player)
    {
        if (player == null)
        {
            return false;
        }

        FindReferences();

        if (box != null && box.Size != BoxSize.Big)
        {
            return false;
        }

        if (HasHolder(player))
        {
            return true;
        }

        if (!TryGetHoldSide(player.transform.position, out BigBoxHoldSide side))
        {
            Debug.Log("[BigBoxCarryController] Big boxes can only be held from left or right.");
            return false;
        }

        if (side == BigBoxHoldSide.Left)
        {
            if (leftHolder != null)
            {
                Debug.Log("[BigBoxCarryController] Left side is already held.");
                return false;
            }

            leftHolder = player;
        }
        else
        {
            if (rightHolder != null)
            {
                Debug.Log("[BigBoxCarryController] Right side is already held.");
                return false;
            }

            rightHolder = player;
        }

        StopMovement();
        return true;
    }

    public void Detach(Player player)
    {
        if (player == null)
        {
            return;
        }

        if (leftHolder == player)
        {
            leftHolder = null;
        }

        if (rightHolder == player)
        {
            rightHolder = null;
        }

        player.ClearExternalVelocity();

        if (!IsReadyToMove)
        {
            StopMovement();
        }
    }

    public void SetMovementFrom(Player player, Vector2 moveInput, float speed)
    {
        if (!HasHolder(player))
        {
            return;
        }

        if (!CanControlMovement(player))
        {
            if (IsPlayerOne(player))
            {
                StopMovement();
            }

            return;
        }

        Vector2 velocity = moveInput == Vector2.zero
            ? Vector2.zero
            : moveInput.normalized * Mathf.Max(0f, speed);

        pendingVelocity = GetAllowedVelocity(velocity);
        ApplyHolderVelocity(pendingVelocity);
    }

    public bool HasHolder(Player player)
    {
        return player != null && (leftHolder == player || rightHolder == player);
    }

    public bool CanControlMovement(Player player)
    {
        return IsReadyToMove && HasHolder(player) && IsPlayerOne(player);
    }

    public bool TryGetFacingDirection(Player player, out Vector2 direction)
    {
        if (leftHolder == player)
        {
            direction = Vector2.right;
            return true;
        }

        if (rightHolder == player)
        {
            direction = Vector2.left;
            return true;
        }

        direction = Vector2.down;
        return false;
    }

    bool TryGetHoldSide(Vector3 playerPosition, out BigBoxHoldSide side)
    {
        Vector2 offset = playerPosition - transform.position;

        if (Mathf.Abs(offset.x) < minHorizontalSeparation)
        {
            side = BigBoxHoldSide.Left;
            return false;
        }

        if (requireHorizontalDominance && Mathf.Abs(offset.x) < Mathf.Abs(offset.y))
        {
            side = BigBoxHoldSide.Left;
            return false;
        }

        side = offset.x < 0f ? BigBoxHoldSide.Left : BigBoxHoldSide.Right;
        return true;
    }

    bool IsPlayerOne(Player player)
    {
        return player != null
            && string.Equals(player.playerID, "P1", System.StringComparison.OrdinalIgnoreCase);
    }

    void StopMovement()
    {
        pendingVelocity = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        ApplyHolderVelocity(Vector2.zero);
    }

    Vector2 GetAllowedVelocity(Vector2 velocity)
    {
        if (velocity == Vector2.zero)
        {
            return Vector2.zero;
        }

        Vector2 direction = velocity.normalized;
        float distance = velocity.magnitude * Time.fixedDeltaTime + Mathf.Max(0f, movementCastPadding);

        if (IsRigidbodyBlocked(rb, direction, distance)
            || IsPlayerBlocked(leftHolder, direction, distance)
            || IsPlayerBlocked(rightHolder, direction, distance))
        {
            return Vector2.zero;
        }

        return velocity;
    }

    bool IsPlayerBlocked(Player player, Vector2 direction, float distance)
    {
        if (player == null)
        {
            return false;
        }

        Rigidbody2D playerRb = player.rb != null ? player.rb : player.GetComponent<Rigidbody2D>();
        return IsRigidbodyBlocked(playerRb, direction, distance);
    }

    bool IsRigidbodyBlocked(Rigidbody2D targetRb, Vector2 direction, float distance)
    {
        if (targetRb == null)
        {
            return false;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(GetBlockingLayerMask());

        int hitCount = targetRb.Cast(direction, filter, castHits, distance);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = castHits[i].collider;

            if (hitCollider == null || hitCollider.isTrigger || IsGroupCollider(hitCollider))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    bool IsGroupCollider(Collider2D target)
    {
        if (target == null)
        {
            return false;
        }

        if (rb != null && target.attachedRigidbody == rb)
        {
            return true;
        }

        Player hitPlayer = target.GetComponentInParent<Player>();
        return hitPlayer != null && (hitPlayer == leftHolder || hitPlayer == rightHolder);
    }

    LayerMask GetBlockingLayerMask()
    {
        if (blockingLayers.value != 0)
        {
            return blockingLayers;
        }

        return LayerMask.GetMask("Wall", "IndivisibleWall", "Fire", "Box", "Player");
    }

    void ApplyHolderVelocity(Vector2 velocity)
    {
        if (leftHolder != null)
        {
            leftHolder.SetExternalVelocity(velocity);
        }

        if (rightHolder != null)
        {
            rightHolder.SetExternalVelocity(velocity);
        }
    }

    void ClearHolderVelocity(Player player)
    {
        if (player != null)
        {
            player.ClearExternalVelocity();
        }
    }

    void FindReferences()
    {
        if (box == null)
        {
            box = GetComponent<BoxController>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }
}
