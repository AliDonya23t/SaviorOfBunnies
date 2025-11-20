using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce = 5f;   // ارتفاع پرش
    [SerializeField] float jumpDistance = 1f; // فاصله افقی پرش
    private bool isGrounded = true;            // آیا روی زمین است؟

    private Rigidbody2D rb;

    [Header("Sound & Animation")]
    public AudioSource jumpAudio;       // AudioSource برای صدای پرش
    public Animator animator;           // Animator کاراکتر

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // متد عمومی پرش برای دکمه UI
    public void JumpButtonPressed()
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    // ← متد مشترک پرش
    private void Jump()
    {
        // پرش Rigidbody
        rb.velocity = new Vector2(jumpDistance, jumpForce);
        isGrounded = false;

        // پخش صدا
        if (jumpAudio != null)
            jumpAudio.Play();

        // فعال کردن Trigger انیمیشن
        if (animator != null)
            animator.SetTrigger("Jump");
    }
}
