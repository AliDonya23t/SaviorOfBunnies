using System.Collections.Generic;
using UnityEngine;

public class PlatformPooler : MonoBehaviour
{
    [Header("Platform prefabs (set your prefabs)")]
    public GameObject[] platformPrefabs;

    [Header("Pooling & spawn settings")]
    public int poolSize = 20;
    public Transform spawnParent;
    public float startX = 0f;
    public float spawnAheadDistance = 25f;   // تا چه فاصله‌ای جلوتر از دوربین اسپاون کنه
    public float despawnBuffer = 6f;         // چه مقدار پشت دوربین حذف شود
    public float minY = -2f, maxY = 2f;      // محدوده عمودی پلتفرم‌ها
    public float minSpacing = 2.2f, maxSpacing = 4f; // فاصله بین پلتفرم‌ها (x)

    Camera mainCam;
    Queue<GameObject> pool = new Queue<GameObject>();
    List<GameObject> activeList = new List<GameObject>();
    float lastSpawnX;

    void Start()
    {
        mainCam = Camera.main;
        lastSpawnX = startX;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = platformPrefabs[i % platformPrefabs.Length];
            GameObject go = Instantiate(prefab, Vector3.one * 9999f, Quaternion.identity, spawnParent);
            go.SetActive(false);
            pool.Enqueue(go);
        }

        // اسپاون اولیه برای پر کردن جلو
        for (int i = 0; i < 6; i++)
            SpawnPlatform();
    }

    void Update()
    {
        float camRight = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect;
        float camLeft = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect;

        // اسپاون جلوتر تا وقتی که آخرین اسپاون داخل محدوده spawnAheadDistance نباشد
        while (lastSpawnX < camRight + spawnAheadDistance)
            SpawnPlatform();

        // بررسی برای حذف پلتفرم‌های پشت سر دوربین
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            GameObject go = activeList[i];
            if (go == null) { activeList.RemoveAt(i); continue; }

            // فرض می‌کنیم pivot در مرکز است؛ می‌تونیم از collider bounds هم استفاده کنیم
            float rightEdge = go.transform.position.x + 0.5f; // اگر پلتفرم عرض داشته باشه، مقدار مناسب قرار بده
            if (rightEdge < camLeft - despawnBuffer)
            {
                go.SetActive(false);
                activeList.RemoveAt(i);
                pool.Enqueue(go);
            }
        }
    }

    void SpawnPlatform()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Platform pool empty, increase poolSize.");
            return;
        }

        int idx = Random.Range(0, platformPrefabs.Length);
        GameObject go = pool.Dequeue();

        float spacing = Random.Range(minSpacing, maxSpacing);
        float spawnX = lastSpawnX + spacing;
        float spawnY = Random.Range(minY, maxY);

        go.transform.position = new Vector3(spawnX, spawnY, 0f);
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        // ⇩ ⇩ ⇩ این بلوک اضافه شده تا الماس‌های داخل پلتفرم دوباره فعال شوند ⇩ ⇩ ⇩
        // اگر الماس‌ها داخل پلتفرم غیرفعال شده باشند، آن‌ها را پیدا و فعال می‌کنیم.
        //var diamonds = go.GetComponentsInChildren<Diamond>(true);
        //foreach (var d in diamonds)
        //{
        //    if (d != null && d.gameObject != null)
        //        d.gameObject.SetActive(true);
        //}
        // ⇧ ⇧ ⇧ بلوک اضافه‌شده پایان ⇧ ⇧ ⇧

        activeList.Add(go);
        lastSpawnX = spawnX;
    }
}
