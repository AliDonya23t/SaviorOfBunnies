using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Object NextSceneAsset;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    private int coins = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        BatterySystem.Instance.EnterMine();
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    private void LoadSceneByName(string sceneName)
    {
        CoinsManager.CreateNewInstance();
        CoinsManager.Instance.AddCoins(coins);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextScene()
    {
        LoadSceneByName(NextSceneAsset.name);
    }

    public int GetCoins() => coins;
}
