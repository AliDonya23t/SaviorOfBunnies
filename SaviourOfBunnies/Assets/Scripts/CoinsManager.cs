using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    private static CoinsManager Instance;
    public static int CollectedCoins = 0;
    public static CoinsManager EnsureInstance()
    {
        if (Instance != null) return Instance;
        Instance = FindObjectOfType<CoinsManager>();
        if (Instance != null) return Instance;
        GameObject obj = new GameObject("CoinsManager");
        Instance = obj.AddComponent<CoinsManager>();
        DontDestroyOnLoad(obj);
        Instance.LoadCoins();
        return Instance;        
    }

    private const string COINS_KEY = "PlayerCoins";
    public int Coins { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else if (Instance != this)
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
