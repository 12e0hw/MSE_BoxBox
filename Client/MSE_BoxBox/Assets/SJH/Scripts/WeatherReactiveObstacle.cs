using UnityEngine;

public class WeatherReactiveObstacle : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private FireObstacle fireObstacle;
    [SerializeField] private WaterSlowZone waterSlowZone;
    [SerializeField] private Collider2D obstacleCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Water")]
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Color waterColor = new Color(0.25f, 0.65f, 1f, 0.85f);
    [SerializeField] private bool useWaterLayer = true;
    [SerializeField] private string waterLayerName = "Water";

    private int originalLayer;
    private bool originalColliderIsTrigger;
    private bool originalAnimatorEnabled;
    private bool originalFireObstacleEnabled;
    private bool originalBlocksPath;
    private Sprite originalSprite;
    private Color originalColor;

    void Awake()
    {
        CacheComponents();
        CacheOriginalState();
        ApplyCurrentWeather();
    }

    void OnEnable()
    {
        WeatherUIController.OnWeatherChanged += HandleWeatherChanged;
        ApplyCurrentWeather();
    }

    void OnDisable()
    {
        WeatherUIController.OnWeatherChanged -= HandleWeatherChanged;
    }

    void CacheComponents()
    {
        if (fireObstacle == null)
        {
            fireObstacle = GetComponent<FireObstacle>();
        }

        if (waterSlowZone == null)
        {
            waterSlowZone = GetComponent<WaterSlowZone>();
        }

        if (obstacleCollider == null)
        {
            obstacleCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void CacheOriginalState()
    {
        originalLayer = gameObject.layer;
        originalColliderIsTrigger = obstacleCollider != null && obstacleCollider.isTrigger;
        originalAnimatorEnabled = animator != null && animator.enabled;
        originalFireObstacleEnabled = fireObstacle != null && fireObstacle.enabled;
        originalBlocksPath = fireObstacle != null && fireObstacle.blocksPath;

        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
            originalColor = spriteRenderer.color;
        }
    }

    void HandleWeatherChanged(string weather, string gameEffect)
    {
        ApplyCurrentWeather();
    }

    void ApplyCurrentWeather()
    {
        if (!WeatherUIController.HasWeather)
        {
            SetFireMode();
            return;
        }

        if (WeatherUIController.IsRainy)
        {
            SetWaterMode();
        }
        else
        {
            SetFireMode();
        }
    }

    void SetWaterMode()
    {
        if (fireObstacle != null)
        {
            fireObstacle.enabled = false;
        }

        if (obstacleCollider != null)
        {
            obstacleCollider.enabled = true;
            obstacleCollider.isTrigger = true;
        }

        if (waterSlowZone != null)
        {
            waterSlowZone.enabled = true;
        }

        if (spriteRenderer != null)
        {
            if (waterSprite != null)
            {
                spriteRenderer.sprite = waterSprite;
            }

            spriteRenderer.color = waterColor;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        if (useWaterLayer)
        {
            int waterLayer = LayerMask.NameToLayer(waterLayerName);
            if (waterLayer >= 0)
            {
                gameObject.layer = waterLayer;
            }
        }
    }

    void SetFireMode()
    {
        if (waterSlowZone != null)
        {
            waterSlowZone.enabled = false;
        }

        gameObject.layer = originalLayer;

        if (obstacleCollider != null)
        {
            obstacleCollider.enabled = true;
            obstacleCollider.isTrigger = originalColliderIsTrigger;
        }

        if (fireObstacle != null)
        {
            fireObstacle.blocksPath = originalBlocksPath;
            fireObstacle.enabled = originalFireObstacleEnabled;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.color = originalColor;
        }

        if (animator != null)
        {
            animator.enabled = originalAnimatorEnabled;
        }
    }
}
