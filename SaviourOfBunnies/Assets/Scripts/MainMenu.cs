using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void BtnExitClick()
    {
        Application.Quit();
        Debug.Log("Exit Clicked!!");
    }
    public void BtnStartClick()
    {
        Debug.Log("Start Clicked!!");
    }
}
