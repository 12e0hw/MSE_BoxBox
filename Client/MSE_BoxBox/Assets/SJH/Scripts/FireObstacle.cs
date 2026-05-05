using UnityEngine;

public class FireObstacle : MonoBehaviour
{
    [Header("Obstacle")]
    public bool blocksPath = true;
    public bool destroyOnExtinguish = true;

    private Collider2D fireCollider;

    void Awake()
    {
        fireCollider = GetComponent<Collider2D>();
        ApplyBlockingState();
    }

    void OnValidate()
    {
        fireCollider = GetComponent<Collider2D>();
        ApplyBlockingState();
    }

    public void Extinguish()
    {
        if (destroyOnExtinguish)
        {
            Destroy(gameObject);
            return;
        }

        blocksPath = false;
        ApplyBlockingState();
        gameObject.SetActive(false);
    }

    void ApplyBlockingState()
    {
        if (fireCollider != null)
        {
            fireCollider.isTrigger = !blocksPath;
            fireCollider.enabled = true;
        }
    }
}
