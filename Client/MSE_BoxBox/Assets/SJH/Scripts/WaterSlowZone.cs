using System.Collections.Generic;
using UnityEngine;

public class WaterSlowZone : MonoBehaviour
{
    // Slows players while they stand inside this water area.
    [SerializeField] private float slowMultiplier = 0.55f;
    [SerializeField] private float lifeTime = 40.0f;

    private readonly HashSet<Player> slowedPlayers = new HashSet<Player>();

    void OnValidate()
    {
        slowMultiplier = Mathf.Clamp(slowMultiplier, 0.1f, 1f);
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnEnable()
    {
        // Keep the water layer from physically blocking players.
        int waterLayer = LayerMask.NameToLayer("Water");
        int playerLayer = LayerMask.NameToLayer("Player");

        if (waterLayer >= 0 && playerLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(waterLayer, playerLayer, false);
        }
    }

    void OnDisable()
    {
        // Restore player speed if this zone disappears while players are inside.
        foreach (Player player in slowedPlayers)
        {
            if (player != null)
            {
                player.RemoveMoveSpeedMultiplier(slowMultiplier);
            }
        }

        slowedPlayers.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Apply the slow effect once per player.
        Player player = other.GetComponentInParent<Player>();
        if (player == null || !slowedPlayers.Add(player))
        {
            return;
        }

        player.AddMoveSpeedMultiplier(slowMultiplier);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Remove the slow effect when the player leaves the water.
        Player player = other.GetComponentInParent<Player>();
        if (player == null || !slowedPlayers.Remove(player))
        {
            return;
        }

        player.RemoveMoveSpeedMultiplier(slowMultiplier);
    }
}
