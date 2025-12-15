using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    public HeartUI heartUI;  // reference to UI

    public int health = 2;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
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

    [Header("Audio")]
    public AudioClip puloAudio;
    public AudioClip colisaoAudio;
    public AudioClip gameOverAudio;
    public AudioClip longJumpAudio;
    public AudioClip enemyDeathAudio;

private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;

        heartUI.UpdateHearts(health);   // initialize UI
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (rb.position.y < -20)
        {
            Die();
        }

        SetAnimation(horizontalMovement);
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
            audioSource.PlayOneShot(puloAudio, 0.7f);
        }
        else if (extraJumps > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioSource.PlayOneShot(puloAudio);
            extraJumps--;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
        else
            audioSource.PlayOneShot(colisaoAudio);
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
                audioSource.PlayOneShot(enemyDeathAudio);
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
            audioSource.PlayOneShot(longJumpAudio);
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
        audioSource.PlayOneShot(gameOverAudio);
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
{
    yield return new WaitForSeconds(1f);
    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
}

}
