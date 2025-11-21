using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance { get; private set; }
    private const string COINS_KEY = "PlayerCoins";
    public int Coins { get; private set; }
    void Awake()
    {
        if(Instance == null)
        {
           Instance = this;
           DontDestroyOnLoad(gameObject);
           LoadCoins(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void LoadCoins()
    {
        Coins = PlayerPrefs.GetInt(COINS_KEY, 0);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        PlayerPrefs.SetInt(COINS_KEY, Coins);
        PlayerPrefs.Save();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount) return false;
        Coins -= amount;
        PlayerPrefs.SetInt(COINS_KEY, Coins);
        PlayerPrefs.Save();
        return true;
    }

    public void ResetCoins()
    {
        Coins = 0;
        PlayerPrefs.DeleteKey(COINS_KEY);
        PlayerPrefs.Save();
    }

}
