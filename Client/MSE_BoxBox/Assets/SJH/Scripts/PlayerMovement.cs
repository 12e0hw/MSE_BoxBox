using UnityEngine;

public class PlayerMovement
{
    private Rigidbody2D rb;
    private float normalSpeed = 3f;
    private float carrySpeed = 2f;
    private float dashSpeed = 5f;
    private float exhaustedSpeed = 1.5f;
    private Vector2 pendingVelocity;

    public bool IsMoving
    {
        get
        {
            return pendingVelocity != Vector2.zero;
        }
    }

    public void Configure(Rigidbody2D targetRb, float normal, float carrying, float dashing, float exhausted)
    {
        rb = targetRb;
        normalSpeed = normal;
        carrySpeed = carrying;
        dashSpeed = dashing;
        exhaustedSpeed = exhausted;
    }

    public void Move(Vector2 moveInput, bool isCarrying, bool isDashing, bool isExhausted)
    {
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

        pendingVelocity = moveInput * speed;
    }

    public void ApplyVelocity()
    {
        if (rb == null) return;

        rb.linearVelocity = pendingVelocity;
    }
}
