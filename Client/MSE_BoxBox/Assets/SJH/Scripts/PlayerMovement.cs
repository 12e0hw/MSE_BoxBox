using UnityEngine;

public class PlayerMovement
{
    // Applies player movement speed and outside movement forces.
    private Rigidbody2D rb;
    private float normalSpeed = 3f;
    private float carrySpeed = 2f;
    private float dashSpeed = 5f;
    private float exhaustedSpeed = 1.5f;
    private float speedMultiplier = 1f;

    private Vector2 pendingVelocity;
    private Vector2 externalVelocity;

    public bool IsMoving
    {
        get
        {
            return pendingVelocity != Vector2.zero;
        }
    }

    public void Configure(Rigidbody2D targetRb, float normal, float carrying, float dashing, float exhausted)
    {
        // Cache movement settings from the Player component.
        rb = targetRb;
        normalSpeed = normal;
        carrySpeed = carrying;
        dashSpeed = dashing;
        exhaustedSpeed = exhausted;
    }

    public void Move(Vector2 moveInput, bool isCarrying, bool isDashing, bool isExhausted)
    {
        // Choose the current move speed from the player state.
        float speed = normalSpeed;

        if (isCarrying)
        {
            speed = carrySpeed;
        }

        if (isDashing)
        {
            if (isCarrying)
            {
                speed = dashSpeed * 0.7f;
                if (speed < carrySpeed)
                {
                    speed = carrySpeed;
                }
            }
            else
            {
                speed = dashSpeed;
            }
        }
        else if (isExhausted)
        {
            if (speed > exhaustedSpeed)
            {
                speed = exhaustedSpeed;
            }
        }

        pendingVelocity = moveInput * speed * speedMultiplier;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        // Used by slow zones and other temporary speed effects.
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    public void SetExternalVelocity(Vector2 velocity)
    {
        // Used by conveyor belts, walkways, and carried big boxes.
        externalVelocity = velocity;
    }

    public void ClearExternalVelocity()
    {
        // Stop outside movement when the effect ends.
        externalVelocity = Vector2.zero;
    }

    public void ApplyVelocity()
    {
        // Apply the final velocity in FixedUpdate.
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = pendingVelocity + externalVelocity;
    }
}
