using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public SpriteRenderer sr; 
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (BatterySystem.Instance != null)
            {
                BatterySystem.Instance.EnterMine(); 
            }


            if (pickupSound != null)
            {
                audioSource.clip = pickupSound;
                audioSource.Play();
            }


            gameObject.SetActive(false);
        }
    }

    
    public void ResetBattery()
    {
        gameObject.SetActive(true);
    }
}
