using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    // Controls the visual spray point and spray effect object.
    public Transform sprayPoint;
    public GameObject sprayEffect;
    public Vector2 sprayPointOffset = new Vector2(0.45f, 0f);
    public Vector2 sprayEffectLocalOffset = new Vector2(0.15f, 0f);

    [Header("Extinguisher Visual")]
    public SpriteRenderer bodySpriteRenderer;
    public bool spriteFacesRight = true;
    
    [Header("Sorting Layer")]
    public string frontSortingLayerName = "Item2";
    public string backSortingLayerName = "Default";
    
    private Animator sprayAnimator;

    void Awake()
    {
        FindSprayReferences();
        FindBodySpriteRenderer();
        SetSpraying(false, Vector2.right);
    }

    public void ConfigureSprayOffset(Vector2 offset)
    {
        // Let the player tune where the spray starts from.
        sprayPointOffset = offset;
    }

    public void UpdateHeldDirection(Vector2 direction)
    {
        // Flip the extinguisher body to match the player's facing direction.
        if (direction == Vector2.zero)
        {
            direction = Vector2.down;
        }

        direction.Normalize();

        FindSprayReferences();
        FindBodySpriteRenderer();

        if (bodySpriteRenderer != null)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                bodySpriteRenderer.flipX = spriteFacesRight ? direction.x < 0f : direction.x > 0f;
            }
            else
            {
                bodySpriteRenderer.flipX = false;
            }
        }

        UpdateSprayPoint(direction);
    }
    
    public void SetSpraying(bool active, Vector2 direction)
    {
        // Toggle the spray effect and rotate it toward the aim direction.
        FindSprayReferences();

        if (sprayEffect == null)
        {
            return;
        }

        Vector2 sprayDirection = direction == Vector2.zero ? Vector2.down : direction.normalized;
        UpdateHeldDirection(sprayDirection);

        bool changed = sprayEffect.activeSelf != active;
        if (changed)
        {
            sprayEffect.SetActive(active);

            if (BGM_Manager.instance != null)
            {
                if (active)
                {
                    BGM_Manager.instance.StartExtinguisherSound();
                }
                else
                {
                    BGM_Manager.instance.StopExtinguisherSound();
                }
            }
        }

        if (sprayAnimator != null)
        {
            if (active && changed)
            {
                sprayAnimator.Play(0, 0, 0f);
            }

            sprayAnimator.enabled = active;
        }
    }

    void FindSprayReferences()
    {
        // Auto-find child objects if the prefab fields are not assigned.
        if (sprayPoint == null)
        {
            sprayPoint = transform.Find("SprayPoint");
        }

        if (sprayPoint == null)
        {
            sprayPoint = transform;
        }

        if (sprayEffect == null)
        {
            Transform effect = sprayPoint.Find("SprayEffect");
            if (effect == null)
            {
                effect = transform.Find("SprayEffect");
            }

            if (effect == null && sprayPoint.childCount > 0)
            {
                effect = sprayPoint.GetChild(0);
            }

            if (effect != null)
            {
                sprayEffect = effect.gameObject;
            }
        }

        if (sprayEffect != null && sprayAnimator == null)
        {
            sprayAnimator = sprayEffect.GetComponent<Animator>();
        }

        if (sprayEffect != null)
        {
            sprayEffect.transform.localPosition = sprayEffectLocalOffset;
        }
    }
    
    public void SetHeldSortingLayer(bool isBehindPlayer)
    {
        FindBodySpriteRenderer();

        if (bodySpriteRenderer == null)
        {
            return;
        }

        bodySpriteRenderer.sortingLayerName = isBehindPlayer ? backSortingLayerName : frontSortingLayerName;
    }

    public void ResetSortingLayerAfterDrop()
    {
        FindBodySpriteRenderer();

        if (bodySpriteRenderer == null)
        {
            return;
        }

        bodySpriteRenderer.sortingLayerName = frontSortingLayerName;
    }
    
    void FindBodySpriteRenderer()
    {
        if (bodySpriteRenderer != null)
        {
            return;
        }

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (sprayEffect != null && spriteRenderer.transform.IsChildOf(sprayEffect.transform))
            {
                continue;
            }

            bodySpriteRenderer = spriteRenderer;
            return;
        }
    }

    void UpdateSprayPoint(Vector2 direction)
    {
        // Move and rotate the spray origin to face the current direction.
        if (sprayPoint == null)
        {
            return;
        }

        sprayPoint.position = transform.position + (Vector3)(direction * sprayPointOffset.x);
        sprayPoint.localPosition += Vector3.up * sprayPointOffset.y;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sprayPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
