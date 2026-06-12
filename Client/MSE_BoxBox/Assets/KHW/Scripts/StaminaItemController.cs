using UnityEngine;

public class StaminaItemController : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private float restoreAmount = 30f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                // Restore stamina once, then remove the item.
                player.RestoreStamina(restoreAmount);
                Destroy(gameObject);
            }
        }
    }
}
