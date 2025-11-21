using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Coins UI")]
    public TextMeshProUGUI coinsText;
    [Header("Click Sound")]
    public AudioClip clickSound;      
    private AudioSource audioSource;  

    private void Awake()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void BtnExitClick()
    {
        PlayClickSound();
        Application.Quit();
        
    }

    public void BtnStartClick()
    {
        PlayClickSound();
        SceneManager.LoadScene("Mine");
        
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

}
