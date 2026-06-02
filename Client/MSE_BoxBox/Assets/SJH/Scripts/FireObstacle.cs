using UnityEngine;

public class FireObstacle : MonoBehaviour
{
    // Fire can block paths until the extinguisher removes it.
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
        // Remove the fire, or disable it if the object should stay in the scene.
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
        // Non-blocking fire should only behave like a trigger.
        if (fireCollider != null)
        {
            fireCollider.isTrigger = !blocksPath;
            fireCollider.enabled = true;
        }
    }
}
