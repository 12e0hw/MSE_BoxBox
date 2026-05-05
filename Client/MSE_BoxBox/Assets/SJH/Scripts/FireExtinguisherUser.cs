using UnityEngine;

public class FireExtinguisherUser
{
    private Transform owner;
    private float range = 1.2f;
    private LayerMask fireLayer;

    public void Configure(Transform ownerTransform, float extinguisherRange, LayerMask targetLayer)
    {
        owner = ownerTransform;
        range = extinguisherRange;
        fireLayer = targetLayer;
    }

    public bool TryUse(Vector2 lastMoveDir)
    {
        if (owner == null) return false;

        Vector2 direction = lastMoveDir == Vector2.zero ? Vector2.down : lastMoveDir.normalized;
        Vector2 origin = owner.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, range, fireLayer);

        Debug.DrawRay(origin, direction * range, Color.cyan, 1f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            FireObstacle fire = hit.collider.GetComponentInParent<FireObstacle>();
            if (fire == null) continue;

            fire.Extinguish();
            return true;
        }

        return false;
    }
}
