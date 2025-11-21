using System.Collections;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public enum DiamondType { Blue, Green, Yellow, Purple }

    public DiamondType type = DiamondType.Blue;
    public AudioClip collectSound;      // (اختیاری) صدای جمع‌آوری
    public float vanishDuration = 0.3f; // مدت محو شدن

    private int coinValue => type switch
    {
        DiamondType.Blue => 1,
        DiamondType.Green => 2,
        DiamondType.Yellow => 3,
        DiamondType.Purple => 5,
        _ => 1
    };

    private Collider2D col;
    private SpriteRenderer sr;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isCollecting = false;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (sr != null) originalColor = sr.color;
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // ری‌ست وضعیت وقتی دوباره فعال می‌شود
        isCollecting = false;
        if (sr != null) sr.color = originalColor;
        transform.localScale = originalScale;
        if (col != null) col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (isCollecting) return;
        isCollecting = true;

        if (col != null) col.enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(coinValue);

        if (collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }

        StartCoroutine(VanishAndDisable());
    }

    private IEnumerator VanishAndDisable()
    {
        float elapsed = 0f;
        Color startColor = sr != null ? sr.color : Color.white;
        Vector3 startScale = transform.localScale;

        while (elapsed < vanishDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / vanishDuration;

            if (sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        // اگر صدا هنوز پخشه کمی صبر کن اما نه بی نهایت
        if (audioSource != null && audioSource.isPlaying)
            yield return new WaitForSeconds(0.05f);

        // ❗ به جای Destroy -> غیرفعال کن تا Pool بتواند آن را دوباره استفاده کند
        gameObject.SetActive(false);
    }
}
