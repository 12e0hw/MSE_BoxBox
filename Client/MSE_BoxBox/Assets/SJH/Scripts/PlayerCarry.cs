using UnityEngine;

public class PlayerCarry
{
    public bool IsCarrying { get; private set; }
    public GameObject CurrentCarriedObject => currentBox;
    public bool IsCarryingExtinguisher => IsExtinguisher(currentBox);

    private Transform playerTransform;
    private Transform carryPoint;
    private SpriteRenderer playerSpriteRenderer;
    private float pickDistance = 0.8f;
    private LayerMask boxLayer;
    private Vector3 frontCarryLocalPos;
    private Vector3 backCarryLocalPos;
    private Vector3 sideCarryLocalPos;
    private GameObject currentBox;
    private Rigidbody2D currentBoxRigidbody;
    private Collider2D currentBoxCollider;
    private SpriteRenderer currentBoxSpriteRenderer;
    private int originalBoxSortingOrder;
    private bool hasOriginalBoxSortingOrder;

    public void Configure(
        Transform owner,
        Transform carryTarget,
        SpriteRenderer ownerSpriteRenderer,
        float distance,
        LayerMask boxTargetLayer,
        Vector3 frontPosition,
        Vector3 backPosition,
        Vector3 sidePosition)
    {
        playerTransform = owner;
        carryPoint = carryTarget;
        playerSpriteRenderer = ownerSpriteRenderer;
        pickDistance = distance;
        boxLayer = boxTargetLayer;
        frontCarryLocalPos = frontPosition;
        backCarryLocalPos = backPosition;
        sideCarryLocalPos = sidePosition;
    }

    public void ToggleCarry(Vector2 lastMoveDir)
    {
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

    Debug.Log("Picked up: " + currentBox.name);
}

public void DropBox(Vector2 lastMoveDir)
{
    if (currentBox == null) return;

    currentBox.transform.SetParent(null);

    Vector3 dropOffset = (Vector3)lastMoveDir.normalized * 0.8f;
    currentBox.transform.position = playerTransform.position + dropOffset;

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
    public void DestroyCarriedObject()
    {
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
    }

    void UpdateCarrySortingOrder(PlayerFacingDirection direction)
    {
        if (!IsCarrying || currentBoxSpriteRenderer == null || playerSpriteRenderer == null) return;

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
}
