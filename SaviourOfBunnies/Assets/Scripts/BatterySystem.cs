using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BatterySystem : MonoBehaviour
{
    public static BatterySystem Instance;

    [Header("Settings")]
    public float testBatteryDuration = 120f; // 2 دقیقه برای تست

    [Header("UI")]
    public Image batteryImage;      // Image برای نمایش باتری
    public Sprite[] batterySprites; // 6 تصویر باتری از پر تا خالی

    private float currentBattery;
    private bool inMine = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentBattery = testBatteryDuration;
        UpdateBatteryUI();
    }

    private void Update()
    {
        if (!inMine) return;

        // کم کردن باتری
        currentBattery -= Time.deltaTime;
        if (currentBattery <= 0f)
        {
            currentBattery = 0f;
            inMine = false;
            GoToMenu();
        }

        UpdateBatteryUI();
    }

    private void GoToMenu()
    {
        // لود منوی اصلی
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateBatteryUI()
    {
        if (batteryImage != null && batterySprites.Length > 0)
        {
            // محاسبه اندیس Sprite
            int index = Mathf.FloorToInt((currentBattery / testBatteryDuration) * (batterySprites.Length - 1));
            batteryImage.sprite = batterySprites[index];
        }
    }

    // فراخوانی وقتی بازیکن وارد معدن شد
    public void EnterMine()
    {
        inMine = true;
        currentBattery = testBatteryDuration;
        UpdateBatteryUI();
    }
}
