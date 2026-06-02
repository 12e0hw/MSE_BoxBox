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

    private const float directionalBlockTolerance = 0.01f;

    private BoxController box;
    private Rigidbody2D rb;
    private Player leftHolder;
    private Player rightHolder;
    private Vector2 requestedVelocity;
    private Vector2 currentVelocity;
    private RigidbodyType2D originalBodyType;
    private bool hasOriginalBodyType;
    private bool isIgnoringPlayerBoxCollisions;
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[32];
    private readonly Collider2D[] attachedColliders = new Collider2D[8];

    private static int playerBoxCollisionIgnoreCount;
    private static bool originalPlayerBoxCollisionIgnored;

    public bool IsReadyToMove => leftHolder != null && rightHolder != null;
    public bool IsHeld => leftHolder != null || rightHolder != null;
    public Vector2 CurrentVelocity => currentVelocity;

    void Awake()
    {
        FindReferences();
    }

    void FixedUpdate()
    {
        FindReferences();

        if (!IsReadyToMove)
        {
            requestedVelocity = Vector2.zero;
            currentVelocity = Vector2.zero;
        }
        else
        {
            currentVelocity = GetAllowedVelocity(requestedVelocity);
        }

        if (rb != null)
        {
            if (IsHeld)
            {
                MoveHeldGroup(currentVelocity);
            }
            else
            {
                rb.linearVelocity = currentVelocity;
            }
        }
    }

    void OnDisable()
    {
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);
        RestoreRigidbody();
    }

    void OnDestroy()
    {
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);
        RestoreRigidbody();
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
            return false;
        }

        if (side == BigBoxHoldSide.Left)
        {
            if (leftHolder != null)
            {
                return false;
            }

            leftHolder = player;
        }
        else
        {
            if (rightHolder != null)
            {
                return false;
            }

            rightHolder = player;
        }

        HoldRigidbody();
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

        if (!IsHeld)
        {
            RestoreRigidbody();
            StopMovement();
        }
        else if (!IsReadyToMove)
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

        requestedVelocity = velocity;
        currentVelocity = GetAllowedVelocity(requestedVelocity);
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
        if (player == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(player.playerID))
        {
            string id = player.playerID.Trim();
            return string.Equals(id, "P1", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(id, "Player1", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(id, "1", System.StringComparison.OrdinalIgnoreCase);
        }

        return player.myPlayerIndex == 1;
    }

    void StopMovement()
    {
        requestedVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        ApplyHolderVelocity(Vector2.zero);
    }

    void HoldRigidbody()
    {
        if (rb == null)
        {
            return;
        }

        if (!hasOriginalBodyType)
        {
            originalBodyType = rb.bodyType;
            hasOriginalBodyType = true;
        }

        // Held big boxes should move only from the carry controller.
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        BeginIgnorePlayerBoxCollisions();
    }

    void RestoreRigidbody()
    {
        EndIgnorePlayerBoxCollisions();

        if (rb == null || !hasOriginalBodyType)
        {
            return;
        }

        rb.bodyType = originalBodyType;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        hasOriginalBodyType = false;
    }

    void MoveHeldGroup(Vector2 velocity)
    {
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (velocity == Vector2.zero)
        {
            return;
        }

        Vector2 delta = velocity * Time.fixedDeltaTime;
        MoveRigidbodyBy(rb, delta);
        MovePlayerBy(leftHolder, delta);
        MovePlayerBy(rightHolder, delta);
    }

    void MovePlayerBy(Player player, Vector2 delta)
    {
        if (player == null)
        {
            return;
        }

        player.ClearExternalVelocity();
        MoveRigidbodyBy(GetPlayerRigidbody(player), delta);
    }

    void MoveRigidbodyBy(Rigidbody2D targetRb, Vector2 delta)
    {
        if (targetRb == null)
        {
            return;
        }

        targetRb.position += delta;
        targetRb.linearVelocity = Vector2.zero;
        targetRb.angularVelocity = 0f;
    }

    Vector2 GetAllowedVelocity(Vector2 velocity)
    {
        if (velocity == Vector2.zero)
        {
            return Vector2.zero;
        }

        Vector2 direction = velocity.normalized;
        float distance = velocity.magnitude * Time.fixedDeltaTime + Mathf.Max(0f, movementCastPadding);

        if (IsRigidbodyBlocked(rb, direction, distance, GetBlockingLayerMask())
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

        Rigidbody2D playerRb = GetPlayerRigidbody(player);
        return IsRigidbodyBlocked(playerRb, direction, distance, GetHolderBlockingLayerMask());
    }

    bool IsRigidbodyBlocked(Rigidbody2D targetRb, Vector2 direction, float distance, LayerMask layerMask)
    {
        if (targetRb == null)
        {
            return false;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(layerMask);

        bool hasTargetBounds = TryGetRigidbodyBounds(targetRb, out Bounds targetBounds);
        int hitCount = targetRb.Cast(direction, filter, castHits, distance);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = castHits[i].collider;

            if (hitCollider == null || hitCollider.isTrigger || IsGroupCollider(hitCollider))
            {
                continue;
            }

            if (castHits[i].distance <= directionalBlockTolerance
                && hasTargetBounds
                && !IsAheadOfMovement(targetBounds, hitCollider.bounds, direction))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    bool TryGetRigidbodyBounds(Rigidbody2D targetRb, out Bounds bounds)
    {
        bounds = new Bounds(Vector3.zero, Vector3.zero);

        if (targetRb == null)
        {
            return false;
        }

        bounds = new Bounds(targetRb.position, Vector3.zero);
        int colliderCount = targetRb.GetAttachedColliders(attachedColliders);
        bool hasBounds = false;

        for (int i = 0; i < colliderCount; i++)
        {
            Collider2D targetCollider = attachedColliders[i];

            if (targetCollider == null || targetCollider.isTrigger)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = targetCollider.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(targetCollider.bounds);
            }
        }

        return hasBounds;
    }

    bool IsAheadOfMovement(Bounds targetBounds, Bounds hitBounds, Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
            {
                return hitBounds.min.x >= targetBounds.max.x - directionalBlockTolerance;
            }

            return hitBounds.max.x <= targetBounds.min.x + directionalBlockTolerance;
        }

        if (direction.y > 0f)
        {
            return hitBounds.min.y >= targetBounds.max.y - directionalBlockTolerance;
        }

        return hitBounds.max.y <= targetBounds.min.y + directionalBlockTolerance;
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

    Rigidbody2D GetPlayerRigidbody(Player player)
    {
        if (player == null)
        {
            return null;
        }

        return player.rb != null ? player.rb : player.GetComponent<Rigidbody2D>();
    }

    void BeginIgnorePlayerBoxCollisions()
    {
        if (isIgnoringPlayerBoxCollisions)
        {
            return;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        int boxLayer = LayerMask.NameToLayer("Box");

        if (playerLayer < 0 || boxLayer < 0)
        {
            return;
        }

        if (playerBoxCollisionIgnoreCount == 0)
        {
            originalPlayerBoxCollisionIgnored = Physics2D.GetIgnoreLayerCollision(playerLayer, boxLayer);
            Physics2D.IgnoreLayerCollision(playerLayer, boxLayer, true);
        }

        playerBoxCollisionIgnoreCount++;
        isIgnoringPlayerBoxCollisions = true;
    }

    void EndIgnorePlayerBoxCollisions()
    {
        if (!isIgnoringPlayerBoxCollisions)
        {
            return;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        int boxLayer = LayerMask.NameToLayer("Box");

        if (playerLayer >= 0 && boxLayer >= 0)
        {
            playerBoxCollisionIgnoreCount = Mathf.Max(0, playerBoxCollisionIgnoreCount - 1);

            if (playerBoxCollisionIgnoreCount == 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, boxLayer, originalPlayerBoxCollisionIgnored);
            }
        }

        isIgnoringPlayerBoxCollisions = false;
    }

    LayerMask GetBlockingLayerMask()
    {
        if (blockingLayers.value != 0)
        {
            return blockingLayers;
        }

        return LayerMask.GetMask("Wall", "IndivisibleWall", "Fire", "Box", "Player");
    }

    LayerMask GetHolderBlockingLayerMask()
    {
        if (blockingLayers.value != 0)
        {
            int boxLayer = LayerMask.NameToLayer("Box");

            if (boxLayer >= 0)
            {
                return blockingLayers.value & ~(1 << boxLayer);
            }

            return blockingLayers;
        }

        return LayerMask.GetMask("Wall", "IndivisibleWall", "Fire", "Player");
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
