using UnityEngine;

public class FireExtinguisherUser
{
    public bool showGauge = true;
    public Vector2 gaugeWorldOffset = new Vector2(0f, 0.9f);
    public Vector2 gaugeSize = new Vector2(120f, 12f);
    public Vector2 sprayPointOffset = new Vector2(0.45f, 0f);

    private Transform owner;
    private float range = 1.2f;
    private float holdSeconds = 3f;
    private float aimDotThreshold = 0.7f;
    private LayerMask fireLayer;
    private float holdTimer;
    private Extinguisher currentExtinguisher;
    private bool isUsing;

    public void Configure(Transform ownerTransform, float extinguisherRange, float requiredHoldSeconds, LayerMask targetLayer)
    {
        owner = ownerTransform;
        range = extinguisherRange;
        holdSeconds = Mathf.Max(0.1f, requiredHoldSeconds);
        fireLayer = targetLayer;
    }

    public void SetCarriedExtinguisher(GameObject extinguisher)
    {
        Extinguisher nextExtinguisher = extinguisher != null ? extinguisher.GetComponent<Extinguisher>() : null;

        if (currentExtinguisher == nextExtinguisher)
        {
            return;
        }

        SetSprayActive(false, Vector2.down);

        currentExtinguisher = nextExtinguisher;

        if (currentExtinguisher != null)
        {
            currentExtinguisher.ConfigureSprayOffset(sprayPointOffset);
            SetSprayActive(false, Vector2.down);
        }
    }

    public bool Tick(float deltaTime, bool useHeld, Vector2 lastMoveDir)
    {
        if (owner == null)
        {
            ResetUse();
            return false;
        }

        Vector2 direction = lastMoveDir == Vector2.zero ? Vector2.down : lastMoveDir.normalized;

        if (!useHeld)
        {
            ResetUse();
            return false;
        }

        FireObstacle targetFire = FindFireInDirection(direction);
        isUsing = targetFire != null;
        UpdateSprayEffect(isUsing, direction);

        if (!isUsing)
        {
            holdTimer = 0f;
            return false;
        }

        holdTimer += deltaTime;

        if (holdTimer < holdSeconds)
        {
            return false;
        }

        targetFire.Extinguish();
        ResetUse();
        return true;
    }

    public void DrawGUI()
    {
        if (!showGauge || holdTimer <= 0f)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null || owner == null)
        {
            return;
        }

        float percent = Mathf.Clamp01(holdTimer / holdSeconds);
        Vector3 worldPosition = owner.position + (Vector3)gaugeWorldOffset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
        {
            return;
        }

        float x = screenPosition.x - gaugeSize.x * 0.5f;
        float y = Screen.height - screenPosition.y - gaugeSize.y * 0.5f;
        Rect backRect = new Rect(x, y, gaugeSize.x, gaugeSize.y);
        Rect fillRect = new Rect(x, y, gaugeSize.x * percent, gaugeSize.y);

        GUI.color = Color.black;
        GUI.DrawTexture(backRect, Texture2D.whiteTexture);

        GUI.color = Color.cyan;
        GUI.DrawTexture(fillRect, Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    FireObstacle FindFireInDirection(Vector2 direction)
    {
        Vector2 origin = owner.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, fireLayer);

        Debug.DrawRay(origin, direction * range, Color.cyan, 1f);

        FireObstacle closestFire = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            FireObstacle fire = hit.GetComponentInParent<FireObstacle>();
            if (fire == null || !fire.isActiveAndEnabled) continue;

            Vector2 firePoint = hit.ClosestPoint(origin);
            if (firePoint == origin)
            {
                firePoint = fire.transform.position;
            }

            Vector2 toFire = firePoint - origin;
            float distance = toFire.magnitude;

            if (distance <= 0.01f || distance > range)
            {
                continue;
            }

            float facingAmount = Vector2.Dot(direction, toFire.normalized);
            if (facingAmount < aimDotThreshold)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFire = fire;
            }
        }

        return closestFire;
    }

    void UpdateSprayEffect(bool active, Vector2 direction)
    {
        SetSprayActive(active, direction);
    }

    void SetSprayActive(bool active, Vector2 direction)
    {
        if (currentExtinguisher != null)
        {
            currentExtinguisher.SetSpraying(active, direction);
        }
    }

    void ResetUse()
    {
        holdTimer = 0f;
        isUsing = false;
        SetSprayActive(false, Vector2.down);
    }
}
