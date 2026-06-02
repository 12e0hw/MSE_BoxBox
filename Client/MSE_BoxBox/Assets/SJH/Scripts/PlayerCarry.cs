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
        }
        else
        {
            DropBox(lastMoveDir);
        }
    }

    public void TryPickUpBox(Vector2 lastMoveDir)
    {
        ValidateState();

        if (playerTransform == null || carryPoint == null) return;

        Vector2 direction = lastMoveDir.normalized;
        Vector2 origin = (Vector2)playerTransform.position + direction * 0.6f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, pickDistance);

        Debug.DrawRay(origin, direction * pickDistance, Color.red, 1f);

        GameObject target = null;
        Collider2D targetCollider = null;
        Rigidbody2D targetRigidbody = null;
        
        int boxLayerIndex = LayerMask.NameToLayer("Box");

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            Rigidbody2D hitRb = hit.collider.attachedRigidbody;
            GameObject candidate;

            if (hitRb != null)
            {
                candidate = hitRb.gameObject;
            }
            else
            {
                candidate = hit.collider.gameObject;
            }

            if (candidate == playerTransform.gameObject)
            {
                continue;
            }

            if (candidate.GetComponent<Player>() != null)
            {
                continue;
            }
            
            // 들 수 있는 물체를 box와 extinguisher로 제한
            bool isBoxLayer =
                candidate.layer == boxLayerIndex ||
                hit.collider.gameObject.layer == boxLayerIndex;

            bool isExtinguisher =
                IsExtinguisher(candidate) ||
                IsExtinguisher(hit.collider.gameObject);

            /*
            bool isBoxLayer = IsInLayerMask(hit.collider.gameObject.layer, boxLayer);
            bool isExtinguisher = IsExtinguisher(candidate);
            */
            if (!isBoxLayer && !isExtinguisher)
            {
                continue;
            }

            target = candidate;
            targetCollider = hit.collider;
            targetRigidbody = hitRb;
            break;
        }

        if (target == null)
        {
            Debug.Log("Carry target not found");
            return;
        }

        BoxController boxController = GetBoxController(target);

        if (boxController != null && boxController.IsBig)
        {
            TryPickUpBigBox(boxController, lastMoveDir);
            return;
        }

        currentBox = target;
    currentBoxCollider = targetCollider;
    currentBoxRigidbody = targetRigidbody;

    if (currentBoxCollider != null)
    {
        currentBoxCollider.enabled = false;
    }

    // 자식 오브젝트에서 SpriteRenderer를 찾도록 변경
    currentBoxSpriteRenderer = currentBox.GetComponentInChildren<SpriteRenderer>();
    
    if (currentBoxSpriteRenderer != null)
    {
        originalBoxSortingOrder = currentBoxSpriteRenderer.sortingOrder;
        hasOriginalBoxSortingOrder = true;
    }

    // isKinematic을 true로 변경
    if (currentBoxRigidbody != null)
    {
        currentBoxRigidbody.bodyType = RigidbodyType2D.Kinematic;
        currentBoxRigidbody.linearVelocity = Vector2.zero;
        currentBoxRigidbody.angularVelocity = 0f;
    }

    currentBox.transform.SetParent(carryPoint);
    currentBox.transform.localPosition = Vector3.zero;
    
    // 회전값도 초기화
    currentBox.transform.localRotation = Quaternion.identity; 

    IsCarrying = true;
    UpdateCarryPointPosition(lastMoveDir);
    UpdateCarriedObjectLocalPosition();

    Debug.Log("Picked up: " + currentBox.name);
}

public void DropBox(Vector2 lastMoveDir)
{
    ValidateState();

    if (currentBigBox != null)
    {
        ReleaseBigBox();
        return;
    }

    if (currentBox == null) return;

    Vector2 dropDirection = lastMoveDir == Vector2.zero ? Vector2.down : lastMoveDir.normalized;
    Vector3 dropPosition = playerTransform.position + (Vector3)(dropDirection * 0.8f);

    if (IsDropPositionBlocked(dropPosition))
    {
        Debug.Log("Cannot drop here: blocked by obstacle or wall");
        return;
    }

    currentBox.transform.SetParent(null);
    currentBox.transform.position = dropPosition;

    if (currentBoxCollider != null)
    {
        currentBoxCollider.enabled = true;
    }

    // 내려놓을 때 물리 엔진이 다시 작동하도록 복구
    if (currentBoxRigidbody != null)
    {
        currentBoxRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    if (currentBoxSpriteRenderer != null && hasOriginalBoxSortingOrder)
    {
        currentBoxSpriteRenderer.sortingOrder = originalBoxSortingOrder;
    }

    currentBox = null;
    currentBoxCollider = null;
    currentBoxRigidbody = null;
    currentBoxSpriteRenderer = null;
    hasOriginalBoxSortingOrder = false;
    IsCarrying = false;
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

    bool IsSameRoot(Transform target, Transform root)
    {
        if (target == null || root == null)
        {
            return false;
        }

        return target == root || target.IsChildOf(root) || root.IsChildOf(target);
    }

    public void DestroyCarriedObject()
    {
        if (currentBigBox != null)
        {
            ReleaseBigBox();
            return;
        }

        if (currentBox == null) return;

        GameObject target = currentBox;

        currentBox = null;
        currentBoxCollider = null;
        currentBoxRigidbody = null;
        currentBoxSpriteRenderer = null;
        hasOriginalBoxSortingOrder = false;
        IsCarrying = false;

        Object.Destroy(target);
    }

    public void UpdateCarryPointPosition(Vector2 lastMoveDir)
    {
        if (carryPoint == null) return;

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
            Vector3 pos = sideCarryLocalPos;
            pos.x = lastMoveDir.x < 0f ? -Mathf.Abs(sideCarryLocalPos.x) : Mathf.Abs(sideCarryLocalPos.x);
            carryPoint.localPosition = pos;
        }

        UpdateCarrySortingOrder(direction);
        UpdateCarriedObjectLocalPosition();
    }

    void UpdateCarriedObjectLocalPosition()
    {
        if (!IsCarrying || currentBox == null) return;

        if (currentBigBox != null)
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
        if (!IsCarrying || currentBoxSpriteRenderer == null || playerSpriteRenderer == null) return;

        if (currentBigBox != null)
        {
            return;
        }

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
/*
    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
*/
    bool IsExtinguisher(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        return target.CompareTag("Extinguisher");
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
        Debug.Log("Grabbed big box: " + currentBox.name);
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
}
