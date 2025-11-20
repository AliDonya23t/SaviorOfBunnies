using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
