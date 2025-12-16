using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{

    public HeartUI heartUI;  // reference to UI

    public int health = 2;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float airAcceleration = 0.05f;
    public float maxAirSpeed = 5f;
    private float horizontalMovement;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    public Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int extraJumpsValue = 1;
    public int extraJumps;

    [Header("Grapple")]
    public Tilemap grappleTilemap;
    public float grappleMaxDistance = 6f;

    private DistanceJoint2D grappleJoint;
    private LineRenderer rope;
    private bool isGrappling;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;

        heartUI.UpdateHearts(health);

        grappleJoint = gameObject.AddComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        grappleJoint.autoConfigureDistance = false;
        grappleJoint.enableCollision = true;

        rope = gameObject.AddComponent<LineRenderer>();
        rope.positionCount = 2;
        rope.startWidth = 0.05f;
        rope.endWidth = 0.05f;
        rope.enabled = false;
        rope.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (!isGrappling)
        {
            if (isGrounded)
            {
                if (horizontalMovement != 0)
                {
                    rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
            }
        }
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (rb.position.y < -20)
        {
            Die();
        }

        SetAnimation(horizontalMovement);

        if (isGrappling)
        {
            rope.SetPosition(0, transform.position);
            rope.SetPosition(1, grappleJoint.connectedAnchor);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (extraJumps > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            extraJumps--;
        }
    }

    public void Grapple(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryStartGrapple();
        }

        if (context.canceled)
        {
            ReleaseGrapple();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!isGrappling)
        {
            if (!isGrounded && horizontalMovement != 0)
            {
                float targetSpeed = horizontalMovement * maxAirSpeed;
                float currentSpeed = rb.linearVelocity.x;

                bool shouldAccelerate = (Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed)) || 
                                        (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed));

                if (shouldAccelerate)
                {
                    float desiredAcceleration = horizontalMovement * airAcceleration;

                    float forceToApply = desiredAcceleration * rb.mass; 

                    rb.AddForce(new Vector2(forceToApply, 0f), ForceMode2D.Force); 
                }
            }
        }
    }

    private void SetAnimation(float moveInput) 
    {

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("PrincessIdle");
            }
            else
            {
                animator.Play("PrincessRight");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("PrincessJump");
            }
            else
            {
                animator.Play("PrincessFall");
            }
        }
    }

    private void TakeDamage()
    {
        health--;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        StartCoroutine(BlinkRed());

        heartUI.UpdateHearts(health);

        if (health <= 0)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

            if (rb.linearVelocity.y < 0f && transform.position.y > enemy.transform.position.y + 0.2f)
            {
                enemy.TakeDamage();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.stompBounceForce); // bounce upward
            }
            else
            {
                TakeDamage();
            }
        }

        if (collision.gameObject.tag == "Damage")
        {
            TakeDamage();
        }
        else if (collision.gameObject.tag == "BouncePad")
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void TryStartGrapple()
    {
        if (grappleTilemap == null) return;

        Vector3Int playerCell = grappleTilemap.WorldToCell(transform.position);
        int searchRadius = Mathf.CeilToInt(grappleMaxDistance);

        for (int x = -searchRadius; x <= searchRadius; x++)
        {
            for (int y = -searchRadius; y <= searchRadius; y++)
            {
                Vector3Int cell = playerCell + new Vector3Int(x, y, 0);
                TileBase tile = grappleTilemap.GetTile(cell);

                if (tile != null)
                {
                    Vector2 worldPos = grappleTilemap.GetCellCenterWorld(cell);
                    float dist = Vector2.Distance(transform.position, worldPos);

                    if (dist <= grappleMaxDistance)
                    {
                        AttachGrapple(worldPos);
                        return;
                    }
                }
            }
        }
    }

    void AttachGrapple(Vector2 anchor)
    {
        isGrappling = true;
        extraJumps = extraJumpsValue;

        grappleJoint.enabled = true;
        grappleJoint.connectedAnchor = anchor;
        grappleJoint.distance = Vector2.Distance(transform.position, anchor);

        rope.enabled = true;
    }

    void ReleaseGrapple()
    {
        isGrappling = false;
        grappleJoint.enabled = false;
        rope.enabled = false;
    }
}
