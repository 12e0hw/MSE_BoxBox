using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("State")]
    private bool isCarrying = false;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Animation")]
    public string characterPrefix = "Man"; // Man 또는 Woman

    [Header("Move")]
    public float moveSpeed = 3f;

    [Header("Keys")]
    public Key upKey;
    public Key downKey;
    public Key leftKey;
    public Key rightKey;
    public Key pickKey;

    [Header("Carry Settings")]
    public Transform carryPoint;     // 박스를 붙일 위치
    public float pickDistance = 0.8f; // Raycast 길이
    public LayerMask boxLayer;        // Box 레이어만 검사

    [Header("Carry Position")]
    public Vector3 frontCarryLocalPos = new Vector3(0f, 0.2f, 0f);
    public Vector3 backCarryLocalPos = new Vector3(0f, 0.35f, 0f);
    public Vector3 sideCarryLocalPos = new Vector3(0.2f, 0.25f, 0f);

    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    private string currentAnim;
    private GameObject currentBox;

    void Update()
    {
        HandleInput();
        UpdateCarryPointPosition();
        HandleCarryInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void HandleInput()
    {
        if (Keyboard.current == null) return;

        float x = 0f;
        float y = 0f;

        if (Keyboard.current[leftKey].isPressed) x = -1f;
        if (Keyboard.current[rightKey].isPressed) x = 1f;
        if (Keyboard.current[upKey].isPressed) y = 1f;
        if (Keyboard.current[downKey].isPressed) y = -1f;

        moveInput = new Vector2(x, y).normalized;

        if (moveInput != Vector2.zero)
        {
            lastMoveDir = moveInput;
        }
    }

    void HandleCarryInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[pickKey].wasPressedThisFrame)
        {
            if (!isCarrying)
                TryPickUpBox();
            else
                DropBox();
        }
    }

    void TryPickUpBox()
    {
        Vector2 direction = lastMoveDir.normalized;
        Vector2 origin = (Vector2)transform.position + direction * 0.6f;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, pickDistance, boxLayer);

        Debug.DrawRay(origin, direction * pickDistance, Color.red, 1f);

        if (hit.collider == null)
        {
            Debug.Log("박스 못 찾음");
            return;
        }


        Rigidbody2D hitRb = hit.collider.attachedRigidbody;
         
         if (hitRb.gameObject == gameObject)
        {
            Debug.Log("자기 자신을 맞아서 무시");
            return;
        }

        currentBox = hitRb.gameObject;

        Collider2D boxCol = currentBox.GetComponent<Collider2D>();
        if (boxCol != null)
        {
            boxCol.enabled = false;
        }

        hitRb.linearVelocity = Vector2.zero;
        hitRb.angularVelocity = 0f;

        currentBox.transform.SetParent(carryPoint);
        currentBox.transform.localPosition = Vector3.zero;

        isCarrying = true;

        Debug.Log("집은 박스: " + currentBox.name);
    }

    void DropBox()
    {
        if (currentBox == null) return;

        currentBox.transform.SetParent(null);

  
        Vector3 dropOffset = (Vector3)lastMoveDir.normalized * 0.8f;
        currentBox.transform.position = transform.position + dropOffset;

        Collider2D boxCol = currentBox.GetComponent<Collider2D>();

        if (boxCol != null)
        {
            boxCol.enabled = true;
        }

        currentBox = null;
        isCarrying = false;
    }

    void UpdateCarryPointPosition()
    {
        if (carryPoint == null) return;

        string direction = GetDirection();

        if (direction == "Front")
        {
            carryPoint.localPosition = frontCarryLocalPos;
        }
        else if (direction == "Back")
        {
            carryPoint.localPosition = backCarryLocalPos;
        }
        else
        {
            Vector3 pos = sideCarryLocalPos;

            if (lastMoveDir.x < 0)
                pos.x = -Mathf.Abs(sideCarryLocalPos.x);
            else
                pos.x = Mathf.Abs(sideCarryLocalPos.x);

            carryPoint.localPosition = pos;
        }
    }

    void UpdateAnimation()
    {
        string direction = GetDirection();
        string state;

        if (direction == "Side")
        {
            if (lastMoveDir.x < 0)
                spriteRenderer.flipX = true;
            else if (lastMoveDir.x > 0)
                spriteRenderer.flipX = false;
        }

        if (isCarrying)
        {
            state = moveInput == Vector2.zero ? "CarryingIdle" : "CarryingWalk";
        }
        else
        {
            state = moveInput == Vector2.zero ? "Idle" : "Walk";
        }

        string animName = characterPrefix + "_" + direction + "_" + state;
        PlayAnimation(animName);
    }

    string GetDirection()
    {
        if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
        {
            return "Side";
        }
        else
        {
            if (lastMoveDir.y > 0)
                return "Back";
            else
                return "Front";
        }
    }

    void PlayAnimation(string animName)
    {
        if (currentAnim == animName) return;

        currentAnim = animName;
        animator.Play(animName);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 dir = Application.isPlaying ? lastMoveDir.normalized : Vector2.down;
        Vector2 origin = transform.position;
        Vector2 end = origin + dir * pickDistance;

        Gizmos.DrawLine(origin, end);
        Gizmos.DrawWireSphere(end, 0.05f);
    }
}