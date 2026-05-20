using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public Transform sprayPoint;
    public GameObject sprayEffect;
    public Vector2 sprayPointOffset = new Vector2(0.45f, 0f);
    public Vector2 sprayEffectLocalOffset = new Vector2(0.15f, 0f);

    private Animator sprayAnimator;

    void Awake()
    {
        FindSprayReferences();
        SetSpraying(false, Vector2.right);
    }

    public void ConfigureSprayOffset(Vector2 offset)
    {
        sprayPointOffset = offset;
    }

    public void SetSpraying(bool active, Vector2 direction)
    {
        FindSprayReferences();

        if (sprayEffect == null)
        {
            return;
        }

        Vector2 sprayDirection = direction == Vector2.zero ? Vector2.down : direction.normalized;
        UpdateSprayPoint(sprayDirection);

        bool changed = sprayEffect.activeSelf != active;
        if (changed)
        {
            sprayEffect.SetActive(active);
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

    void UpdateSprayPoint(Vector2 direction)
    {
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
