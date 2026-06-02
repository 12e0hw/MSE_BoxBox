using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerID = "P1";

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

    [Header("Name")]
    public TextMeshProUGUI myNameText;
    public int myPlayerIndex = 1;

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
    public bool IsHoldingBigBox => carry != null && carry.IsHoldingBigBox;
    public float CurrentStamina => stamina != null ? stamina.CurrentStamina : maxStamina;
    public float MaxStamina => stamina != null ? stamina.MaxStamina : maxStamina;

    private PlayerInputHandler inputHandler;
    private PlayerMovement movement;
    private PlayerCarry carry;
    private PlayerStamina stamina;
    private PlayerAnimationController animationController;
    private FireExtinguisherUser extinguisherUser;
    private readonly List<float> moveSpeedMultipliers = new List<float>();

    private Coroutine npcSlowCoroutine;
    private bool isNpcSlowActive;
    private float currentNpcSlowMultiplier = 1f;

    void Awake()
    {
        FindComponents();
        SyncSettingsToComponents();
        UpdateKey();
        RefreshName();
    }

    void OnEnable()
    {
        ChangeKey.OnkeyChanged += UpdateKey;
    }

    void OnDisable()
    {
        ChangeKey.OnkeyChanged -= UpdateKey;
        ClearNpcSlow();
    }

    void Update()
    {
        inputHandler.ReadInput();
        carry.ValidateState();

        // Run player features in a fixed order.
        HandleCarryInput();
        UpdateExtinguisherUse();
        UpdateMovementAndAnimation();
    }

    void FixedUpdate()
    {
        movement.ApplyVelocity();
    }

    void OnGUI()
    {
        if (stamina != null)
        {
            stamina.guiPosition = GetStaminaGuiPosition();
            stamina.DrawGUI();
        }

        if (extinguisherUser != null)
        {
            extinguisherUser.DrawGUI();
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 direction = Application.isPlaying && inputHandler != null ? inputHandler.LastMoveDir.normalized : Vector2.down;
        Vector2 origin = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + direction * pickDistance);
        Gizmos.DrawWireSphere(origin + direction * pickDistance, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + direction * extinguisherRange);
    }

    public void RefreshName()
    {
        if (myNameText == null)
        {
            return;
        }

        string savedName = PlayerPrefs.GetString("Player" + myPlayerIndex + "_Name", "기본이름");
        myNameText.text = savedName;
    }

    public void SetExternalVelocity(Vector2 velocity)
    {
        if (movement == null)
        {
            return;
        }

        movement.SetExternalVelocity(velocity);
    }

    public void ClearExternalVelocity()
    {
        if (movement == null)
        {
            return;
        }

        movement.ClearExternalVelocity();
    }

    public void AddMoveSpeedMultiplier(float multiplier)
    {
        moveSpeedMultipliers.Add(NormalizeMoveSpeedMultiplier(multiplier));
        ApplyMoveSpeedMultipliers();
    }

    public void RemoveMoveSpeedMultiplier(float multiplier)
    {
        float normalizedMultiplier = NormalizeMoveSpeedMultiplier(multiplier);
        int index = moveSpeedMultipliers.FindIndex(value => Mathf.Approximately(value, normalizedMultiplier));

        if (index >= 0)
        {
            moveSpeedMultipliers.RemoveAt(index);
        }

        ApplyMoveSpeedMultipliers();
    }

    public void ApplyNpcSlow(float slowMultiplier, float duration)
    {
        slowMultiplier = NormalizeMoveSpeedMultiplier(slowMultiplier);
        duration = Mathf.Max(0f, duration);

        if (npcSlowCoroutine != null)
        {
            StopCoroutine(npcSlowCoroutine);
            npcSlowCoroutine = null;
        }

        if (isNpcSlowActive)
        {
            RemoveMoveSpeedMultiplier(currentNpcSlowMultiplier);
        }

        currentNpcSlowMultiplier = slowMultiplier;
        isNpcSlowActive = true;

        AddMoveSpeedMultiplier(currentNpcSlowMultiplier);
        npcSlowCoroutine = StartCoroutine(NpcSlowRoutine(duration));
    }

    public void RestoreStamina(float amount)
    {
        if (stamina != null)
        {
            stamina.Restore(amount);
        }
    }

    // Load saved key settings for each player.
    void UpdateKey()
    {
        if (playerID == "P1")
        {
            upKey = ChangeKey.GetSavedKey("P1_UpKey", Key.W);
            downKey = ChangeKey.GetSavedKey("P1_DownKey", Key.S);
            leftKey = ChangeKey.GetSavedKey("P1_LeftKey", Key.A);
            rightKey = ChangeKey.GetSavedKey("P1_RightKey", Key.D);
            interactKey = ChangeKey.GetSavedKey("P1_InteractKey", Key.C);
            extinguisherKey = ChangeKey.GetSavedKey("P1_FireKey", Key.V);
            dashKey = ChangeKey.GetSavedKey("P1_RunKey", Key.B);
        }
        else if (playerID == "P2")
        {
            upKey = ChangeKey.GetSavedKey("P2_UpKey", Key.UpArrow);
            downKey = ChangeKey.GetSavedKey("P2_DownKey", Key.DownArrow);
            leftKey = ChangeKey.GetSavedKey("P2_LeftKey", Key.LeftArrow);
            rightKey = ChangeKey.GetSavedKey("P2_RightKey", Key.RightArrow);
            interactKey = ChangeKey.GetSavedKey("P2_InteractKey", Key.I);
            extinguisherKey = ChangeKey.GetSavedKey("P2_FireKey", Key.O);
            dashKey = ChangeKey.GetSavedKey("P2_RunKey", Key.P);
        }

        if (inputHandler != null)
        {
            inputHandler.Configure(upKey, downKey, leftKey, rightKey, interactKey, extinguisherKey, dashKey);
        }
    }

    void HandleCarryInput()
    {
        if (inputHandler.InteractPressed)
        {
            carry.ToggleCarry(inputHandler.LastMoveDir);
        }
    }

    void UpdateExtinguisherUse()
    {
        bool isUsingExtinguisher = inputHandler.ExtinguisherHeld && carry.IsCarryingExtinguisher;
        extinguisherUser.SetCarriedExtinguisher(carry.CurrentCarriedObject);

        if (extinguisherUser.Tick(Time.deltaTime, isUsingExtinguisher, inputHandler.LastMoveDir))
        {
            carry.DestroyCarriedObject();
        }
    }

    void UpdateMovementAndAnimation()
    {
        Vector2 moveInput = inputHandler.MoveInput;

        if (carry.IsHoldingBigBox)
        {
            carry.UpdateBigBoxMovement(moveInput, carryMoveSpeed);
        }

        Vector2 playerMoveInput = carry.IsHoldingBigBox ? Vector2.zero : moveInput;
        Vector2 animationMoveInput = playerMoveInput;
        Vector2 facingDirection = inputHandler.LastMoveDir;

        if (carry.IsHoldingBigBox && carry.IsBigBoxReadyToMove)
        {
            animationMoveInput = carry.BigBoxVelocity == Vector2.zero ? Vector2.zero : carry.BigBoxVelocity.normalized;
        }

        if (carry.IsHoldingBigBox)
        {
            facingDirection = carry.BigBoxFacingDirection;
        }

        bool isMoving = carry.IsHoldingBigBox
            ? carry.BigBoxVelocity != Vector2.zero
            : playerMoveInput != Vector2.zero;
        bool wantsDash = inputHandler.DashHeld && isMoving && !carry.IsHoldingBigBox;

        stamina.Tick(Time.deltaTime, isMoving, carry.IsCarrying, wantsDash);

        bool isDashing = wantsDash && stamina.CanDash;
        carry.UpdateCarryPointPosition(facingDirection);
        movement.Move(playerMoveInput, carry.IsCarrying, isDashing, stamina.IsExhausted);
        animationController.UpdateAnimation(animationMoveInput, facingDirection, carry.IsCarrying);
    }

    void ApplyMoveSpeedMultipliers()
    {
        if (movement == null)
        {
            return;
        }

        float multiplier = 1f;

        foreach (float value in moveSpeedMultipliers)
        {
            multiplier = Mathf.Min(multiplier, value);
        }

        movement.SetSpeedMultiplier(multiplier);
    }

    IEnumerator NpcSlowRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        ClearNpcSlow();
    }

    void ClearNpcSlow()
    {
        if (npcSlowCoroutine != null)
        {
            StopCoroutine(npcSlowCoroutine);
            npcSlowCoroutine = null;
        }

        if (!isNpcSlowActive)
        {
            return;
        }

        RemoveMoveSpeedMultiplier(currentNpcSlowMultiplier);

        currentNpcSlowMultiplier = 1f;
        isNpcSlowActive = false;
    }

    float NormalizeMoveSpeedMultiplier(float multiplier)
    {
        return Mathf.Clamp(multiplier, 0.1f, 1f);
    }

    void SyncSettingsToComponents()
    {
        // Send inspector settings to helper classes.
        movement.Configure(rb, moveSpeed, carryMoveSpeed, dashMoveSpeed, exhaustedMoveSpeed);
        ApplyMoveSpeedMultipliers();

        carry.Configure(
            this,
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
            float rightMargin = Mathf.Max(0f, staminaGuiPosition.x);
            float topMargin = Mathf.Max(0f, staminaGuiPosition.y);
            float x = Screen.width - staminaGuiSize.x - rightMargin;

            return new Vector2(x, topMargin);
        }

        return staminaGuiPosition;
    }
}
