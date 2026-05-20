using UnityEngine;

public class PlayerMovement
{
    private Rigidbody2D rb;
    private float normalSpeed = 3f;
    private float carrySpeed = 2f;
    private float dashSpeed = 5f;
    private float exhaustedSpeed = 1.5f;
    
    private Vector2 pendingVelocity;
    // 무빙워크 발판 속도 추가
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
    
    // 무빙워크 밟았을 때 속도관여 함수
    public void SetExternalVelocity(Vector2 velocity)
    {
        externalVelocity = velocity;
    }

    public void ClearExternalVelocity()
    {
        externalVelocity = Vector2.zero;
    }
    
    public void ApplyVelocity()
    {
        if (rb == null) return;

        rb.linearVelocity = pendingVelocity + externalVelocity;
    }
}
