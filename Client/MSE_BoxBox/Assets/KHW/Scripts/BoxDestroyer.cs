using UnityEngine;

public class BoxDestroyer : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            // Remove objects that enter the cleanup area.
            Destroy(collision.gameObject);
        }
    }
}
