using UnityEngine;

public class NPCFsmMove : MonoBehaviour
{
    private enum NPCState
    {
        RandomMove,
        BlockPlayer,
        Cooldown
    }

    [Header("State")]
    [SerializeField] private NPCState currentState = NPCState.RandomMove;

    [Header("Move Settings")]
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float randomMoveMinTime = 0.5f;
    [SerializeField] private float randomMoveMaxTime = 1f;
    [SerializeField] private float randomIdleMinTime = 0.5f;
    [SerializeField] private float randomIdleMaxTime = 1f;

    [Header("Player Slow")]
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float slowDuration = 2f;
    
    [Header("Player Block Settings")]
    [SerializeField] private Transform[] players;
    [SerializeField] private float detectRange = 4f;
    [SerializeField] private float blockSpeed = 2f;
    [SerializeField] private float stopDistanceFromPlayer = 0.7f;
    [SerializeField] private float maxBlockTime = 2f;

    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 3f;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveDir;
    private float currentSpeed;

    private float stateTimer;
    private float actionTimer;

    private bool isRandomMoving;
    private Transform targetPlayer;

    private bool hasIsMovingParameter;
    private bool hasHorizontalParameter;
    private bool hasVerticalParameter;
    
    private readonly Vector2[] directions =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim != null)
        {
            hasIsMovingParameter = HasAnimatorParameter("isMoving", AnimatorControllerParameterType.Bool);
            hasHorizontalParameter = HasAnimatorParameter("Horizontal", AnimatorControllerParameterType.Float);
            hasVerticalParameter = HasAnimatorParameter("Vertical", AnimatorControllerParameterType.Float);
        }
    }

    private void Start()
    {
        ChangeState(NPCState.RandomMove);
    }

    private void Update()
    {
        switch (currentState)
        {
            case NPCState.RandomMove:
                UpdateRandomMoveState();
                break;

            case NPCState.BlockPlayer:
                UpdateBlockPlayerState();
                break;

            case NPCState.Cooldown:
                UpdateCooldownState();
                break;
        }
    }
    
    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (moveDir == Vector2.zero)
        {
            return;
        }

        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
    }

    // Update random movement and switch to blocking if a player is nearby.
    private void UpdateRandomMoveState()
    {
        targetPlayer = FindNearestPlayerInRange();

        if (targetPlayer != null)
        {
            ChangeState(NPCState.BlockPlayer);
            return;
        }

        actionTimer -= Time.deltaTime;

        if (actionTimer > 0f)
        {
            return;
        }

        if (isRandomMoving)
        {
            StartRandomIdle();
        }
        else
        {
            StartRandomMove();
        }
    }

    // Move toward the target player and block their path.
    private void UpdateBlockPlayerState()
    {
        stateTimer += Time.deltaTime;

        if (targetPlayer == null)
        {
            ChangeState(NPCState.Cooldown);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer > detectRange)
        {
            ChangeState(NPCState.Cooldown);
            return;
        }

        if (stateTimer >= maxBlockTime)
        {
            ChangeState(NPCState.Cooldown);
            return;
        }

        if (distanceToPlayer <= stopDistanceFromPlayer)
        {
            moveDir = Vector2.zero;
            SetWalking(false);
            return;
        }

        Vector2 directionToPlayer = targetPlayer.position - transform.position;
        moveDir = directionToPlayer.normalized;
        currentSpeed = blockSpeed;

        SetWalking(true);
    }
    
    // Wait during cooldown before returning to random movement.
    private void UpdateCooldownState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= cooldownTime)
        {
            ChangeState(NPCState.RandomMove);
        }
    }

    // Start moving in a random direction.
    private void StartRandomMove()
    {
        moveDir = directions[Random.Range(0, directions.Length)];
        currentSpeed = Random.Range(1f, 1.5f) * baseSpeed;

        isRandomMoving = true;
        actionTimer = Random.Range(randomMoveMinTime, randomMoveMaxTime);

        SetWalking(true);
    }

    // Stop moving for a random idle duration.
    private void StartRandomIdle()
    {
        moveDir = Vector2.zero;

        isRandomMoving = false;
        actionTimer = Random.Range(randomIdleMinTime, randomIdleMaxTime);

        SetWalking(false);
    }

    // Change the NPC state and reset state timers.
    private void ChangeState(NPCState nextState)
    {
        currentState = nextState;
        stateTimer = 0f;
        actionTimer = 0f;

        switch (currentState)
        {
            case NPCState.RandomMove:
                StartRandomIdle();
                break;

            case NPCState.BlockPlayer:
                SetWalking(true);
                break;

            case NPCState.Cooldown:
                StopMovementAndAnimation();
                break;
        }
    }

    // Find the nearest player within the detection range.
    private Transform FindNearestPlayerInRange()
    {
        Transform nearestPlayer = null;
        float nearestDistance = detectRange;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, players[i].position);

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = players[i];
            }
        }

        return nearestPlayer;
    }

    // Update walking animation and sprite direction.
    private void SetWalking(bool isMoving)
    {
        if (anim != null)
        {
            anim.speed = isMoving ? 1f : 0f;

            if (hasIsMovingParameter)
            {
                anim.SetBool("isMoving", isMoving);
            }
        }

        if (!isMoving)
        {
            return;
        }

        Vector2 animationDirection = GetAnimationDirection(moveDir);

        if (anim != null)
        {
            if (hasHorizontalParameter)
            {
                anim.SetFloat("Horizontal", animationDirection.x);
            }

            if (hasVerticalParameter)
            {
                anim.SetFloat("Vertical", animationDirection.y);
            }
        }

        if (spriteRenderer != null)
        {
            if (animationDirection.x < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (animationDirection.x > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    // Convert movement direction into animation direction.
    private Vector2 GetAnimationDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return new Vector2(Mathf.Sign(direction.x), 0f);
        }

        if (Mathf.Abs(direction.y) > 0f)
        {
            return new Vector2(0f, Mathf.Sign(direction.y));
        }

        return Vector2.down;
    }

    // Slow the player on collision or bounce back from obstacles.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == NPCState.Cooldown)
        {
            return;
        }

        Player player = collision.gameObject.GetComponent<Player>();

        if (player == null)
        {
            player = collision.gameObject.GetComponentInParent<Player>();
        }

        if (player != null)
        {
            player.ApplyNpcSlow(slowMultiplier, slowDuration);
            ChangeState(NPCState.Cooldown);
            return;
        }

        BounceBack();
    }

    // Reverse movement direction after hitting an obstacle.
    private void BounceBack()
    {
        moveDir = -moveDir;

        if (moveDir == Vector2.zero)
        {
            moveDir = directions[Random.Range(0, directions.Length)];
        }

        currentSpeed = baseSpeed;
        SetWalking(true);
    }
    
    // Stop NPC movement and animation.
    private void StopMovementAndAnimation()
    {
        moveDir = Vector2.zero;
        currentSpeed = 0f;
        SetWalking(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
    
    // Check whether the animator has the required parameter.
    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (anim == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}