using UnityEngine;
using System.Collections;

public class NPC_move : MonoBehaviour
{
    [Header("Settings")]
    public float baseSpeed = 1f; 
    
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDir;
    
    private float currentSpeed; 
    
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Coroutine moveCoroutine;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            moveDir = directions[Random.Range(0, directions.Length)];
            currentSpeed = Random.Range(1f, 1.5f) * baseSpeed;
            SetWalking(true);

            yield return new WaitForSeconds(Random.Range(0.5f,1f));

            moveDir = Vector2.zero;
            SetWalking(false);

            yield return new WaitForSeconds(Random.Range(0.5f,1f));
        }
    }

    void SetWalking(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            anim.SetFloat("Horizontal", moveDir.x);
            anim.SetFloat("Vertical", moveDir.y);
            if (moveDir.x < 0) spriteRenderer.flipX = true;
            else if (moveDir.x > 0) spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDir = -moveDir;
    
        if (moveDir == Vector2.zero)
        {
            moveDir = directions[Random.Range(0, directions.Length)];
        }

        SetWalking(true);
        
        moveCoroutine = StartCoroutine(MoveAfterCollision());
    }


    IEnumerator MoveAfterCollision()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        moveCoroutine = StartCoroutine(MoveRoutine());
    }
}