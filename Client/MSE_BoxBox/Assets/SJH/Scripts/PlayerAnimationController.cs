using UnityEngine;

public enum PlayerFacingDirection
{
    Front,
    Back,
    Side
}

public class PlayerAnimationController
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private string characterPrefix = "Man";
    private string currentAnim;

    public void Configure(Animator targetAnimator, SpriteRenderer targetSpriteRenderer, string prefix)
    {
        animator = targetAnimator;
        spriteRenderer = targetSpriteRenderer;
        characterPrefix = prefix;
    }

    public void UpdateAnimation(Vector2 moveInput, Vector2 lastMoveDir, bool isCarrying)
    {
        if (animator == null) return;

        PlayerFacingDirection direction = GetDirection(lastMoveDir);

        if (direction == PlayerFacingDirection.Side && spriteRenderer != null)
        {
            if (lastMoveDir.x < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (lastMoveDir.x > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }

        string state;
        if (isCarrying)
        {
            state = moveInput == Vector2.zero ? "CarryingIdle" : "CarryingWalk";
        }
        else
        {
            state = moveInput == Vector2.zero ? "Idle" : "Walk";
        }

        string animName = characterPrefix + "_" + DirectionName(direction) + "_" + state;
        PlayAnimation(animName);
    }

    void PlayAnimation(string animName)
    {
        if (currentAnim == animName) return;

        currentAnim = animName;
        animator.Play(animName);
    }

    PlayerFacingDirection GetDirection(Vector2 lastMoveDir)
    {
        if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
        {
            return PlayerFacingDirection.Side;
        }

        if (lastMoveDir.y > 0f)
        {
            return PlayerFacingDirection.Back;
        }

        return PlayerFacingDirection.Front;
    }

    string DirectionName(PlayerFacingDirection direction)
    {
        if (direction == PlayerFacingDirection.Back)
        {
            return "Back";
        }

        if (direction == PlayerFacingDirection.Side)
        {
            return "Side";
        }

        return "Front";
    }
}
