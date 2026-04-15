using UnityEngine;
using System.Collections;

public class NPC_move : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 1f; 
    
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDir;

    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
    
            moveDir = directions[Random.Range(0, directions.Length)];
            float currentSpeed = Random.Range(0.5f, 2f); // 속도 랜덤
            SetWalking(true);

            yield return new WaitForSeconds(Random.Range(1f, 2f));

            moveDir = Vector2.zero;
            SetWalking(false);

            yield return new WaitForSeconds(Random.Range(1f, 2f));
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
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }
}