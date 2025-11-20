using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackgroundManager : MonoBehaviour
{
    [Header("Background Prefabs (set at least 2 prefabs)")]
    public GameObject[] backgroundPrefabs; // حداقل 2 تا داشته باشی بهتره

    [Header("Pooling & placement")]
    public int poolSize = 5;                // چند بک‌گراند در pool نگه داریم
    public Transform spawnParent;          // اختیاری برای مرتب نگه داشتن در هیراکی
    public float tileWidth = 20f;          // عرض هر بک‌گراند در یونیتی (تعیین‌شده توسط تو)
    public float spawnAheadDistance = 30f; // چقدر جلوتر از دوربین باید بک‌گراند باشه
    public float despawnBuffer = 5f;       // مقدار اضافه برای حذف وقتی که کاملاً خارج شد

    Camera mainCam;
    Queue<GameObject> pool = new Queue<GameObject>();
    List<GameObject> activeList = new List<GameObject>();
    float lastSpawnX;

    void Start()
    {
        mainCam = Camera.main;

        // محاسبه عرض واقعی از Renderer
        SpriteRenderer sr = backgroundPrefabs[0].GetComponent<SpriteRenderer>();
        tileWidth = sr.bounds.size.x;

        lastSpawnX = mainCam.transform.position.x - tileWidth;

        // ایجاد pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = backgroundPrefabs[i % backgroundPrefabs.Length];
            GameObject go = Instantiate(prefab, Vector3.one * 9999f, Quaternion.identity, spawnParent);
            go.SetActive(false);
            pool.Enqueue(go);
        }

        for (int i = 0; i < 3; i++)
            SpawnTile();
    }

    void Update()
    {
        float camRight = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect;
        float camLeft = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect;

        // اگر آخرین اسپاون هنوز در ناحیه دید جلو نیست، یکی جدید بساز
        while (lastSpawnX < camRight + spawnAheadDistance)
            SpawnTile();

        // بررسی برای despawn (اگر کامل از صفحه خارج شده)
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            GameObject go = activeList[i];
            if (go == null) { activeList.RemoveAt(i); continue; }

            float rightEdge = go.transform.position.x + tileWidth * 0.5f;
            if (rightEdge < camLeft - despawnBuffer)
            {
                // غیرفعال و بازگشت به pool
                go.SetActive(false);
                activeList.RemoveAt(i);
                pool.Enqueue(go);
            }
        }
    }

    void SpawnTile()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Background pool empty, consider increasing poolSize.");
            return;
        }

        GameObject go = pool.Dequeue();
        // انتخاب تصادفی مدل جدید (به‌جای استفاده از pool index)
        // اگر می‌خواهی ترتیب داشته باشی، این خط را تغییر بده
        var prefabIdx = Random.Range(0, backgroundPrefabs.Length);
        // اگر می‌خواهی ظاهر را تغییر دهی می‌توانی sprite را عوض کنی، اما اینجا فرض می‌کنیم prefab آماده است.

        float spawnX = lastSpawnX + tileWidth;
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y, 0f); // y را با transform این manager هماهنگ کن

        go.transform.position = spawnPos;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        activeList.Add(go);
        lastSpawnX = spawnX;
    }
}
