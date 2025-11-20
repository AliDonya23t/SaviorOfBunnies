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

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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
        if (col != null) col.enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(coinValue);

        if (collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }

        StartCoroutine(VanishAndDestroy());
    }

    private IEnumerator VanishAndDestroy()
    {
        float elapsed = 0f;
        Color startColor = sr.color;
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

        if (audioSource != null && audioSource.isPlaying)
            yield return new WaitForSeconds(0.05f);

        Destroy(gameObject);
    }
}
