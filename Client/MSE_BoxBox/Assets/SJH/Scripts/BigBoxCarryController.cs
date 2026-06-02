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
    [SerializeField] private float movementCastPadding = 0f;
    [SerializeField] private bool useMovementBlockCheck = false;
    [SerializeField] private LayerMask blockingLayers;

    private BoxController box;
    private Rigidbody2D rb;
    private Player leftHolder;
    private Player rightHolder;
    private Vector2 pendingVelocity;
    private int lastBoxCollisionRefreshFrame = -1;
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[32];

    public bool IsHeld => leftHolder != null || rightHolder != null;
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

        RefreshMovingBoxCollisionIgnores();

        if (rb != null)
        {
            rb.linearVelocity = pendingVelocity;
        }

        ApplyHolderVelocity(pendingVelocity);
    }

    void OnDisable()
    {
        SetBoxCollisionIgnored(leftHolder, false);
        SetBoxCollisionIgnored(rightHolder, false);
        SetMovingBoxCollisionIgnored(leftHolder, false);
        SetMovingBoxCollisionIgnored(rightHolder, false);
        SetHeldBoxCollisionWithOtherBoxesIgnored(false);
        ClearHolderVelocity(leftHolder);
        ClearHolderVelocity(rightHolder);
    }

    void OnDestroy()
    {
        SetBoxCollisionIgnored(leftHolder, false);
        SetBoxCollisionIgnored(rightHolder, false);
        SetMovingBoxCollisionIgnored(leftHolder, false);
        SetMovingBoxCollisionIgnored(rightHolder, false);
        SetHeldBoxCollisionWithOtherBoxesIgnored(false);
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

        SetBoxCollisionIgnored(player, true);
        RefreshMovingBoxCollisionIgnores();
        StopMovement();
        return true;
    }

    public void Detach(Player player)
    {
        if (player == null)
        {
            return;
        }

        SetBoxCollisionIgnored(player, false);
        SetMovingBoxCollisionIgnored(player, false);

        if (leftHolder == player)
        {
            leftHolder = null;
        }

        if (rightHolder == player)
        {
            rightHolder = null;
        }

        if (!IsHeld)
        {
            SetHeldBoxCollisionWithOtherBoxesIgnored(false);
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
        if (player == null)
        {
            return false;
        }

        string playerId = string.IsNullOrWhiteSpace(player.playerID)
            ? string.Empty
            : player.playerID.Trim();

        if (string.Equals(playerId, "P1", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(playerId, "Player1", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(playerId, "Player 1", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(playerId, "1", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return playerId.Length == 0
            && string.Equals(player.characterPrefix, "Man", System.StringComparison.OrdinalIgnoreCase);
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

        if (!useMovementBlockCheck)
        {
            return velocity;
        }

        LayerMask layerMask = GetBlockingLayerMask();
        if (layerMask.value == 0)
        {
            return velocity;
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

        if (IsHolderOrBoxRigidbody(target.attachedRigidbody))
        {
            return true;
        }

        return IsSameRoot(target.transform, transform)
            || IsSameRoot(target.transform, GetPlayerTransform(leftHolder))
            || IsSameRoot(target.transform, GetPlayerTransform(rightHolder));
    }

    LayerMask GetBlockingLayerMask()
    {
        if (blockingLayers.value != 0)
        {
            return blockingLayers;
        }

        return LayerMask.GetMask("Wall", "IndisibleWall", "IndivisibleWall", "Fire");
    }

    void RefreshMovingBoxCollisionIgnores()
    {
        if (!IsHeld || lastBoxCollisionRefreshFrame == Time.frameCount)
        {
            return;
        }

        lastBoxCollisionRefreshFrame = Time.frameCount;

        SetMovingBoxCollisionIgnored(leftHolder, true);
        SetMovingBoxCollisionIgnored(rightHolder, true);
        SetHeldBoxCollisionWithOtherBoxesIgnored(true);
    }

    void SetBoxCollisionIgnored(Player player, bool ignored)
    {
        if (player == null)
        {
            return;
        }

        Collider2D[] boxColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D boxCollider in boxColliders)
        {
            if (boxCollider == null)
            {
                continue;
            }

            foreach (Collider2D playerCollider in playerColliders)
            {
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(boxCollider, playerCollider, ignored);
                }
            }
        }
    }

    void SetMovingBoxCollisionIgnored(Player player, bool ignored)
    {
        if (player == null)
        {
            return;
        }

        Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>();
        Collider2D[] boxColliders = FindBoxLayerColliders();

        foreach (Collider2D boxCollider in boxColliders)
        {
            if (boxCollider == null || IsSameRoot(boxCollider.transform, player.transform))
            {
                continue;
            }

            foreach (Collider2D playerCollider in playerColliders)
            {
                if (playerCollider != null && playerCollider != boxCollider)
                {
                    Physics2D.IgnoreCollision(playerCollider, boxCollider, ignored);
                }
            }
        }
    }

    void SetHeldBoxCollisionWithOtherBoxesIgnored(bool ignored)
    {
        Collider2D[] heldBoxColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] boxColliders = FindBoxLayerColliders();

        foreach (Collider2D boxCollider in boxColliders)
        {
            if (boxCollider == null || IsSameRoot(boxCollider.transform, transform))
            {
                continue;
            }

            foreach (Collider2D heldBoxCollider in heldBoxColliders)
            {
                if (heldBoxCollider != null && heldBoxCollider != boxCollider)
                {
                    Physics2D.IgnoreCollision(heldBoxCollider, boxCollider, ignored);
                }
            }
        }
    }

    Collider2D[] FindBoxLayerColliders()
    {
        int boxLayer = LayerMask.NameToLayer("Box");
        Collider2D[] colliders = UnityEngine.Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);

        if (boxLayer < 0)
        {
            return colliders;
        }

        int writeIndex = 0;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null && colliders[i].gameObject.layer == boxLayer)
            {
                colliders[writeIndex] = colliders[i];
                writeIndex++;
            }
        }

        System.Array.Resize(ref colliders, writeIndex);
        return colliders;
    }

    bool IsHolderOrBoxRigidbody(Rigidbody2D targetRb)
    {
        return targetRb != null
            && (targetRb == rb
                || targetRb == GetPlayerRigidbody(leftHolder)
                || targetRb == GetPlayerRigidbody(rightHolder));
    }

    Rigidbody2D GetPlayerRigidbody(Player player)
    {
        if (player == null)
        {
            return null;
        }

        return player.rb != null ? player.rb : player.GetComponent<Rigidbody2D>();
    }

    Transform GetPlayerTransform(Player player)
    {
        return player != null ? player.transform : null;
    }

    bool IsSameRoot(Transform target, Transform root)
    {
        if (target == null || root == null)
        {
            return false;
        }

        return target == root || target.IsChildOf(root) || root.IsChildOf(target);
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
