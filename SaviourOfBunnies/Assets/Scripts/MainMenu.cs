using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Coins UI")]
    public TextMeshProUGUI coinsText;

    public void BtnExitClick()
    {
        Application.Quit();
        Debug.Log("Exit Clicked!!");
    }
    public void BtnStartClick()
    {
        SceneManager.LoadScene("Mine");
        Debug.Log("Start Clicked!!");
    }

}
