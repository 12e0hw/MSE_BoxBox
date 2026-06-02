using System.Collections.Generic;
using UnityEngine;

public class WaterSlowZone : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.55f;
    [SerializeField] private float lifeTime = 10.0f;

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
        int waterLayer = LayerMask.NameToLayer("Water");
        int playerLayer = LayerMask.NameToLayer("Player");

        if (waterLayer >= 0 && playerLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(waterLayer, playerLayer, false);
        }
    }

    void OnDisable()
    {
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
        Player player = other.GetComponentInParent<Player>();
        if (player == null || !slowedPlayers.Add(player))
        {
            return;
        }

        player.AddMoveSpeedMultiplier(slowMultiplier);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null || !slowedPlayers.Remove(player))
        {
            return;
        }

        player.RemoveMoveSpeedMultiplier(slowMultiplier);
    }
}
