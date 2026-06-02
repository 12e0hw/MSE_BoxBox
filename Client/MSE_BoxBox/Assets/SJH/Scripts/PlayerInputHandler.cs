using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    // Reads keyboard input and keeps the latest facing direction.
    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; private set; } = Vector2.down;
    public bool InteractPressed { get; private set; }
    public bool ExtinguisherPressed { get; private set; }
    public bool ExtinguisherHeld { get; private set; }
    public bool DashHeld { get; private set; }

    private Key upKey;
    private Key downKey;
    private Key leftKey;
    private Key rightKey;
    private Key interactKey;
    private Key extinguisherKey;
    private Key dashKey;

    public void Configure(Key up, Key down, Key left, Key right, Key interact, Key extinguisher, Key dash)
    {
        // Store the current key bindings for this player.
        upKey = up;
        downKey = down;
        leftKey = left;
        rightKey = right;
        interactKey = interact;
        extinguisherKey = extinguisher;
        dashKey = dash;
    }

    public void ReadInput()
    {
        // Reset one-frame input states before reading the keyboard.
        InteractPressed = false;
        ExtinguisherPressed = false;
        DashHeld = false;

        if (Keyboard.current == null)
        {
            MoveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (IsPressed(leftKey)) x = -1f;
        if (IsPressed(rightKey)) x = 1f;
        if (IsPressed(upKey)) y = 1f;
        if (IsPressed(downKey)) y = -1f;

        MoveInput = new Vector2(x, y).normalized;

        if (MoveInput != Vector2.zero)
        {
            // Keep the last non-zero direction for carrying and aiming.
            LastMoveDir = MoveInput;
        }

        InteractPressed = WasPressedThisFrame(interactKey);
        ExtinguisherPressed = WasPressedThisFrame(extinguisherKey);
        ExtinguisherHeld = IsPressed(extinguisherKey);
        DashHeld = IsPressed(dashKey);
    }

    bool IsPressed(Key key)
    {
        return key != Key.None && Keyboard.current[key].isPressed;
    }

    bool WasPressedThisFrame(Key key)
    {
        return key != Key.None && Keyboard.current[key].wasPressedThisFrame;
    }
}
