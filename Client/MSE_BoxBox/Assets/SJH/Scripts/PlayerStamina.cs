using UnityEngine;

public class PlayerStamina
{
    [Header("Debug UI")]
    public bool showDebugBar = true;
    public Vector2 guiPosition = new Vector2(20f, 20f);
    public Vector2 guiSize = new Vector2(120f, 12f);

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool CanDash => CurrentStamina >= minStaminaToDash;
    public bool IsExhausted => CurrentStamina <= 0f;

    private float maxStamina = 100f;
    private float dashDrainPerSecond = 25f;
    private float carryDrainPerSecond = 8f;
    private float recoverPerSecond = 18f;
    private float minStaminaToDash = 5f;
    private bool recoverWhileCarryingIdle = true;
    private bool initialized;

    public void Configure(float max, float dashDrain, float carryDrain, float recover, float minDash, bool recoverCarryingIdle)
    {
        maxStamina = Mathf.Max(1f, max);
        dashDrainPerSecond = Mathf.Max(0f, dashDrain);
        carryDrainPerSecond = Mathf.Max(0f, carryDrain);
        recoverPerSecond = Mathf.Max(0f, recover);
        minStaminaToDash = Mathf.Clamp(minDash, 0f, maxStamina);
        recoverWhileCarryingIdle = recoverCarryingIdle;

        if (!initialized)
        {
            CurrentStamina = maxStamina;
            initialized = true;
        }

        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    public void Tick(float deltaTime, bool isMoving, bool isCarrying, bool wantsDash)
    {
        float drain = 0f;

        if (isMoving && isCarrying)
        {
            drain += carryDrainPerSecond;
        }

        if (isMoving && wantsDash && CanDash)
        {
            drain += dashDrainPerSecond;
        }

        if (drain > 0f)
        {
            CurrentStamina -= drain * deltaTime;
        }
        else if (!isCarrying || recoverWhileCarryingIdle)
        {
            CurrentStamina += recoverPerSecond * deltaTime;
        }

        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
    }

    public void DrawGUI()
    {
        if (!showDebugBar) return;

        float percent = MaxStamina <= 0f ? 0f : CurrentStamina / MaxStamina;
        percent = Mathf.Clamp01(percent);

        Rect backRect = new Rect(guiPosition.x, guiPosition.y, guiSize.x, guiSize.y);
        Rect fillRect = new Rect(guiPosition.x, guiPosition.y, guiSize.x * percent, guiSize.y);

        GUI.color = Color.black;
        GUI.DrawTexture(backRect, Texture2D.whiteTexture);

        GUI.color = GetFillColor(percent);
        GUI.DrawTexture(fillRect, Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    Color GetFillColor(float percent)
    {
        if (percent > 0.5f)
        {
            return Color.green;
        }

        if (percent > 0.25f)
        {
            return Color.yellow;
        }

        return Color.red;
    }
}
