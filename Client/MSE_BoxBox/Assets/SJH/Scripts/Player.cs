using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Animation")]
    public string characterPrefix = "Man";

    [Header("Move")]
    public float moveSpeed = 3f;
    public float carryMoveSpeed = 2f;
    public float dashMoveSpeed = 5f;
    public float exhaustedMoveSpeed = 1.5f;

    [Header("Keys")]
    public Key upKey;
    public Key downKey;
    public Key leftKey;
    public Key rightKey;
    public Key interactKey;
    public Key extinguisherKey;
    public Key dashKey;

    [Header("Carry Settings")]
    public Transform carryPoint;
    public float pickDistance = 0.8f;
    public LayerMask boxLayer;

    [Header("Carry Position")]
    public Vector3 frontCarryLocalPos = new Vector3(0f, 0.2f, 0f);
    public Vector3 backCarryLocalPos = new Vector3(0f, 0.35f, 0f);
    public Vector3 sideCarryLocalPos = new Vector3(0.2f, 0.25f, 0f);
    public Vector3 extinguisherCarryLocalOffset = new Vector3(0f, -0.25f, 0f);

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float dashDrainPerSecond = 25f;
    public float carryDrainPerSecond = 8f;
    public float recoverPerSecond = 18f;
    public float minStaminaToDash = 5f;
    public bool recoverWhileCarryingIdle = true;

    [Header("Stamina UI")]
    public bool showStaminaBar = true;
    public Vector2 staminaGuiPosition = new Vector2(20f, 20f);
    public Vector2 staminaGuiSize = new Vector2(120f, 12f);

    [Header("Extinguisher")]
    public float extinguisherRange = 1.2f;
    public float extinguishHoldSeconds = 3f;
    public Vector2 extinguisherSprayPointOffset = new Vector2(0.45f, 0f);
    public Vector2 extinguisherGaugeOffset = new Vector2(0f, 0.9f);
    public Vector2 extinguisherGaugeSize = new Vector2(120f, 12f);
    public bool showExtinguisherGauge = true;
    public LayerMask fireLayer;

    public Vector2 MoveInput => inputHandler != null ? inputHandler.MoveInput : Vector2.zero;
    public Vector2 LastMoveDir => inputHandler != null ? inputHandler.LastMoveDir : Vector2.down;
    public bool IsCarrying => carry != null && carry.IsCarrying;
    public float CurrentStamina => stamina != null ? stamina.CurrentStamina : maxStamina;
    public float MaxStamina => stamina != null ? stamina.MaxStamina : maxStamina;

    private PlayerInputHandler inputHandler;
    private PlayerMovement movement;
    private PlayerCarry carry;
    private PlayerStamina stamina;
    private PlayerAnimationController animationController;
    private FireExtinguisherUser extinguisherUser;

    void Awake()
    {
        FindComponents();
        SyncSettingsToComponents();
    }

    void Update()
    {
        inputHandler.ReadInput();

        if (inputHandler.InteractPressed)
        {
            carry.ToggleCarry(inputHandler.LastMoveDir);
        }

        bool isUsingExtinguisher = inputHandler.ExtinguisherHeld && carry.IsCarryingExtinguisher;
        extinguisherUser.SetCarriedExtinguisher(carry.CurrentCarriedObject);
        if (extinguisherUser.Tick(Time.deltaTime, isUsingExtinguisher, inputHandler.LastMoveDir))
        {
            carry.DestroyCarriedObject();
        }

        bool isMoving = inputHandler.MoveInput != Vector2.zero;
        bool wantsDash = inputHandler.DashHeld && isMoving;
        stamina.Tick(Time.deltaTime, isMoving, carry.IsCarrying, wantsDash);

        bool isDashing = wantsDash && stamina.CanDash;
        carry.UpdateCarryPointPosition(inputHandler.LastMoveDir);
        movement.Move(inputHandler.MoveInput, carry.IsCarrying, isDashing, stamina.IsExhausted);
        animationController.UpdateAnimation(inputHandler.MoveInput, inputHandler.LastMoveDir, carry.IsCarrying);
    }

    void FixedUpdate()
    {
        movement.ApplyVelocity();
    }

    void SyncSettingsToComponents()
    {
        inputHandler.Configure(upKey, downKey, leftKey, rightKey, interactKey, extinguisherKey, dashKey);
        movement.Configure(rb, moveSpeed, carryMoveSpeed, dashMoveSpeed, exhaustedMoveSpeed);
        carry.Configure(
            transform,
            carryPoint,
            spriteRenderer,
            pickDistance,
            boxLayer,
            frontCarryLocalPos,
            backCarryLocalPos,
            sideCarryLocalPos,
            extinguisherCarryLocalOffset);
        stamina.Configure(maxStamina, dashDrainPerSecond, carryDrainPerSecond, recoverPerSecond, minStaminaToDash, recoverWhileCarryingIdle);
        stamina.showDebugBar = showStaminaBar;
        stamina.guiPosition = GetStaminaGuiPosition();
        stamina.guiSize = staminaGuiSize;
        animationController.Configure(animator, spriteRenderer, characterPrefix);
        extinguisherUser.Configure(transform, extinguisherRange, extinguishHoldSeconds, fireLayer);
        extinguisherUser.sprayPointOffset = extinguisherSprayPointOffset;
        extinguisherUser.showGauge = showExtinguisherGauge;
        extinguisherUser.gaugeWorldOffset = extinguisherGaugeOffset;
        extinguisherUser.gaugeSize = extinguisherGaugeSize;
    }

    void FindComponents()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        inputHandler = new PlayerInputHandler();
        movement = new PlayerMovement();
        carry = new PlayerCarry();
        stamina = new PlayerStamina();
        animationController = new PlayerAnimationController();
        extinguisherUser = new FireExtinguisherUser();
    }

    Vector2 GetStaminaGuiPosition()
    {
        if (characterPrefix == "Woman")
        {
            return new Vector2(staminaGuiPosition.x, staminaGuiPosition.y + 20f);
        }

        return staminaGuiPosition;
    }

    void OnGUI()
    {
        if (stamina != null)
        {
            stamina.DrawGUI();
        }

        if (extinguisherUser != null)
        {
            extinguisherUser.DrawGUI();
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 dir = Application.isPlaying && inputHandler != null ? inputHandler.LastMoveDir.normalized : Vector2.down;
        Vector2 origin = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + dir * pickDistance);
        Gizmos.DrawWireSphere(origin + dir * pickDistance, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + dir * extinguisherRange);
    }
}
