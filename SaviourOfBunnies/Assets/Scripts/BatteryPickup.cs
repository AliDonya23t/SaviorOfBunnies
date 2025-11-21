using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public SpriteRenderer sr; 
    public AudioClip pickupSound; 

    private void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
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
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            
            gameObject.SetActive(false);
        }
    }

    
    public void ResetBattery()
    {
        gameObject.SetActive(true);
    }
}
