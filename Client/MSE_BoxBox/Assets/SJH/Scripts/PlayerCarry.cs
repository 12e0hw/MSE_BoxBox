using UnityEngine;

public class PlayerCarry
{
    public bool IsCarrying { get; private set; }
    public GameObject CurrentCarriedObject => currentBox;
    public bool IsCarryingExtinguisher => IsExtinguisher(currentBox);
    public bool IsHoldingBigBox => currentBigBox != null;
    public bool IsBigBoxReadyToMove => currentBigBox != null && currentBigBox.IsReadyToMove;
    public bool CanControlBigBoxMovement => currentBigBox != null && currentBigBox.CanControlMovement(ownerPlayer);
    public Vector2 BigBoxVelocity => currentBigBox != null ? currentBigBox.CurrentVelocity : Vector2.zero;

    public Vector2 BigBoxFacingDirection
    {
        get
        {
            if (currentBigBox != null && currentBigBox.TryGetFacingDirection(ownerPlayer, out Vector2 direction))
            {
                return direction;
            }

            return Vector2.down;
        }
    }

    private Player ownerPlayer;
    private Transform playerTransform;
    private Transform carryPoint;
    private SpriteRenderer playerSpriteRenderer;
    private float pickDistance = 0.8f;
    private LayerMask boxLayer;
    private Vector3 frontCarryLocalPos;
    private Vector3 backCarryLocalPos;
    private Vector3 sideCarryLocalPos;
    private Vector3 extinguisherCarryLocalOffset;

    private GameObject currentBox;
    private Rigidbody2D currentBoxRigidbody;
    private Collider2D currentBoxCollider;
    private SpriteRenderer currentBoxSpriteRenderer;
    private int originalBoxSortingOrder;
    private bool hasOriginalBoxSortingOrder;
    private BigBoxCarryController currentBigBox;

    public void Configure(
        Player owner,
        Transform ownerTransform,
        Transform carryTarget,
        SpriteRenderer ownerSpriteRenderer,
        float distance,
        LayerMask boxTargetLayer,
        Vector3 frontPosition,
        Vector3 backPosition,
        Vector3 sidePosition,
        Vector3 extinguisherOffset)
    {
        ownerPlayer = owner;
        playerTransform = ownerTransform != null ? ownerTransform : owner != null ? owner.transform : null;
        carryPoint = carryTarget;
        playerSpriteRenderer = ownerSpriteRenderer;
        pickDistance = distance;
        boxLayer = boxTargetLayer;
        frontCarryLocalPos = frontPosition;
        backCarryLocalPos = backPosition;
        sideCarryLocalPos = sidePosition;
        extinguisherCarryLocalOffset = extinguisherOffset;
    }

    public void ToggleCarry(Vector2 lastMoveDir)
    {
        ValidateState();

        if (!IsCarrying)
        {
            TryPickUpBox(lastMoveDir);
            return;
        }

        DropBox(lastMoveDir);
    }

    public void TryPickUpBox(Vector2 lastMoveDir)
    {
        ValidateState();

        if (playerTransform == null || carryPoint == null)
        {
            return;
        }

        if (!TryFindCarryTarget(lastMoveDir, out GameObject target, out Collider2D targetCollider, out Rigidbody2D targetRigidbody))
        {
            return;
        }

        BoxController boxController = GetBoxController(target);
        if (boxController != null && boxController.IsBig)
        {
            TryPickUpBigBox(boxController, lastMoveDir);
            return;
        }

        PickUpSmallObject(target, targetCollider, targetRigidbody, lastMoveDir);
    }

    public void DropBox(Vector2 lastMoveDir)
    {
        ValidateState();

        if (currentBigBox != null)
        {
            ReleaseBigBox();
            return;
        }

        if (currentBox == null)
        {
            return;
        }

        Vector2 dropDirection = lastMoveDir == Vector2.zero ? Vector2.down : lastMoveDir.normalized;
        Vector3 dropPosition = playerTransform.position + (Vector3)(dropDirection * 0.8f);

        if (IsDropPositionBlocked(dropPosition))
        {
            return;
        }

        currentBox.transform.SetParent(null);
        currentBox.transform.position = dropPosition;

        if (currentBoxCollider != null)
        {
            currentBoxCollider.enabled = true;
        }

        if (currentBoxRigidbody != null)
        {
            currentBoxRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        if (currentBoxSpriteRenderer != null && hasOriginalBoxSortingOrder)
        {
            currentBoxSpriteRenderer.sortingOrder = originalBoxSortingOrder;
        }

        ClearCarryState();
    }

    public void ValidateState()
    {
        if (IsCarrying && currentBox == null)
        {
            ClearCarryState();
        }
    }

    public void UpdateBigBoxMovement(Vector2 moveInput, float speed)
    {
        if (currentBigBox == null)
        {
            return;
        }

        currentBigBox.SetMovementFrom(ownerPlayer, moveInput, speed);
    }

    public void DestroyCarriedObject()
    {
        if (currentBigBox != null)
        {
            ReleaseBigBox();
            return;
        }

        if (currentBox == null)
        {
            return;
        }

        GameObject target = currentBox;
        ClearCarryState();

        Object.Destroy(target);
    }

    public void UpdateCarryPointPosition(Vector2 lastMoveDir)
    {
        if (carryPoint == null)
        {
            return;
        }

        PlayerFacingDirection direction = GetDirection(lastMoveDir);

        if (direction == PlayerFacingDirection.Front)
        {
            carryPoint.localPosition = frontCarryLocalPos;
        }
        else if (direction == PlayerFacingDirection.Back)
        {
            carryPoint.localPosition = backCarryLocalPos;
        }
        else
        {
            Vector3 position = sideCarryLocalPos;
            position.x = lastMoveDir.x < 0f ? -Mathf.Abs(sideCarryLocalPos.x) : Mathf.Abs(sideCarryLocalPos.x);
            carryPoint.localPosition = position;
        }

        UpdateCarrySortingOrder(direction);
        UpdateCarriedObjectLocalPosition();
    }

    bool TryFindCarryTarget(
        Vector2 lastMoveDir,
        out GameObject target,
        out Collider2D targetCollider,
        out Rigidbody2D targetRigidbody)
    {
        // Find the first box or extinguisher in front of the player.
        Vector2 direction = lastMoveDir.normalized;
        Vector2 origin = (Vector2)playerTransform.position + direction * 0.6f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, pickDistance);

        Debug.DrawRay(origin, direction * pickDistance, Color.red, 1f);

        target = null;
        targetCollider = null;
        targetRigidbody = null;

        foreach (RaycastHit2D hit in hits)
        {
            if (!IsValidCarryHit(hit, out GameObject candidate, out Rigidbody2D hitRigidbody))
            {
                continue;
            }

            target = candidate;
            targetCollider = hit.collider;
            targetRigidbody = hitRigidbody;
            return true;
        }

        return false;
    }

    bool IsValidCarryHit(RaycastHit2D hit, out GameObject candidate, out Rigidbody2D hitRigidbody)
    {
        candidate = null;
        hitRigidbody = null;

        if (hit.collider == null)
        {
            return false;
        }

        hitRigidbody = hit.collider.attachedRigidbody;
        candidate = hitRigidbody != null ? hitRigidbody.gameObject : hit.collider.gameObject;

        if (candidate == playerTransform.gameObject || candidate.GetComponent<Player>() != null)
        {
            return false;
        }

        return IsBoxTarget(candidate, hit.collider.gameObject) || IsExtinguisher(candidate) || IsExtinguisher(hit.collider.gameObject);
    }

    bool IsBoxTarget(GameObject candidate, GameObject hitObject)
    {
        int boxLayerIndex = LayerMask.NameToLayer("Box");

        if (boxLayerIndex >= 0)
        {
            return candidate.layer == boxLayerIndex || hitObject.layer == boxLayerIndex;
        }

        return IsInLayerMask(candidate.layer, boxLayer) || IsInLayerMask(hitObject.layer, boxLayer);
    }

    void PickUpSmallObject(GameObject target, Collider2D targetCollider, Rigidbody2D targetRigidbody, Vector2 lastMoveDir)
    {
        // Attach small objects to the carry point.
        currentBox = target;
        currentBoxCollider = targetCollider;
        currentBoxRigidbody = targetRigidbody;
        currentBoxSpriteRenderer = currentBox.GetComponentInChildren<SpriteRenderer>();

        if (currentBoxCollider != null)
        {
            currentBoxCollider.enabled = false;
        }

        if (currentBoxSpriteRenderer != null)
        {
            originalBoxSortingOrder = currentBoxSpriteRenderer.sortingOrder;
            hasOriginalBoxSortingOrder = true;
        }

        if (currentBoxRigidbody != null)
        {
            currentBoxRigidbody.bodyType = RigidbodyType2D.Kinematic;
            currentBoxRigidbody.linearVelocity = Vector2.zero;
            currentBoxRigidbody.angularVelocity = 0f;
        }

        currentBox.transform.SetParent(carryPoint);
        currentBox.transform.localPosition = Vector3.zero;
        currentBox.transform.localRotation = Quaternion.identity;

        IsCarrying = true;
        UpdateCarryPointPosition(lastMoveDir);
        UpdateCarriedObjectLocalPosition();
    }

    bool IsDropPositionBlocked(Vector3 dropPosition)
    {
        Vector2 checkSize = GetDropCheckSize();
        Collider2D[] hits = Physics2D.OverlapBoxAll(dropPosition, checkSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.isTrigger)
            {
                continue;
            }

            if (currentBoxCollider != null && hit == currentBoxCollider)
            {
                continue;
            }

            if (IsSameRoot(hit.transform, currentBox.transform))
            {
                continue;
            }

            if (playerTransform != null && IsSameRoot(hit.transform, playerTransform))
            {
                continue;
            }

            if (hit.gameObject.CompareTag("Truck"))
            {
                continue;
            }

            if (IsBlockingDropLayer(hit.gameObject.layer))
            {
                return true;
            }
        }

        return false;
    }

    Vector2 GetDropCheckSize()
    {
        BoxCollider2D boxCollider = currentBoxCollider as BoxCollider2D;

        if (boxCollider != null)
        {
            Vector3 scale = currentBox.transform.lossyScale;
            return new Vector2(
                Mathf.Abs(boxCollider.size.x * scale.x) * 0.9f,
                Mathf.Abs(boxCollider.size.y * scale.y) * 0.9f);
        }

        if (currentBoxCollider != null)
        {
            Vector2 boundsSize = currentBoxCollider.bounds.size;
            if (boundsSize != Vector2.zero)
            {
                return boundsSize * 0.9f;
            }
        }

        return new Vector2(0.8f, 0.8f);
    }

    bool IsBlockingDropLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Wall")
            || layer == LayerMask.NameToLayer("IndisibleWall")
            || layer == LayerMask.NameToLayer("Fire")
            || layer == LayerMask.NameToLayer("Box")
            || layer == LayerMask.NameToLayer("Player");
    }

    void UpdateCarriedObjectLocalPosition()
    {
        if (!IsCarrying || currentBox == null || currentBigBox != null)
        {
            return;
        }

        if (IsCarryingExtinguisher)
        {
            currentBox.transform.localPosition = extinguisherCarryLocalOffset;
            return;
        }

        currentBox.transform.localPosition = Vector3.zero;
    }

    void UpdateCarrySortingOrder(PlayerFacingDirection direction)
    {
        if (!IsCarrying || currentBoxSpriteRenderer == null || playerSpriteRenderer == null || currentBigBox != null)
        {
            return;
        }

        // Draw carried items in front or behind the player.
        if (direction == PlayerFacingDirection.Front)
        {
            currentBoxSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
        }
        else if (direction == PlayerFacingDirection.Back)
        {
            currentBoxSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
        }
        else if (hasOriginalBoxSortingOrder)
        {
            currentBoxSpriteRenderer.sortingOrder = originalBoxSortingOrder;
        }
    }

    PlayerFacingDirection GetDirection(Vector2 lastMoveDir)
    {
        if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
        {
            return PlayerFacingDirection.Side;
        }

        if (lastMoveDir.y > 0f)
        {
            return PlayerFacingDirection.Back;
        }

        return PlayerFacingDirection.Front;
    }

    bool IsExtinguisher(GameObject target)
    {
        return target != null && target.CompareTag("Extinguisher");
    }

    bool TryPickUpBigBox(BoxController boxController, Vector2 lastMoveDir)
    {
        BigBoxCarryController bigBoxCarry = boxController.GetComponent<BigBoxCarryController>();

        if (bigBoxCarry == null)
        {
            bigBoxCarry = boxController.gameObject.AddComponent<BigBoxCarryController>();
        }

        if (!bigBoxCarry.TryAttach(ownerPlayer))
        {
            return false;
        }

        currentBigBox = bigBoxCarry;
        currentBox = boxController.gameObject;
        currentBoxCollider = null;
        currentBoxRigidbody = null;
        currentBoxSpriteRenderer = null;
        hasOriginalBoxSortingOrder = false;
        IsCarrying = true;

        UpdateCarryPointPosition(lastMoveDir);
        return true;
    }

    void ReleaseBigBox()
    {
        if (currentBigBox != null)
        {
            currentBigBox.Detach(ownerPlayer);
        }

        ClearCarryState();
    }

    void ClearCarryState()
    {
        currentBox = null;
        currentBoxCollider = null;
        currentBoxRigidbody = null;
        currentBoxSpriteRenderer = null;
        currentBigBox = null;
        hasOriginalBoxSortingOrder = false;
        IsCarrying = false;
    }

    BoxController GetBoxController(GameObject target)
    {
        if (target == null)
        {
            return null;
        }

        BoxController box = target.GetComponent<BoxController>();

        if (box == null)
        {
            box = target.GetComponentInParent<BoxController>();
        }

        return box;
    }

    bool IsSameRoot(Transform target, Transform root)
    {
        if (target == null || root == null)
        {
            return false;
        }

        return target == root || target.IsChildOf(root) || root.IsChildOf(target);
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
