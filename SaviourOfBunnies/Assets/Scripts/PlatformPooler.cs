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

            float rightEdge = go.transform.position.x + 0.5f; // فرض pivot در مرکز
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

        // انتخاب index برای prefab (مهم: ما از این idx برای مرجع گرفتن childها استفاده می‌کنیم)
        int idx = Random.Range(0, platformPrefabs.Length);
        GameObject go = pool.Dequeue();

        float spacing = Random.Range(minSpacing, maxSpacing);
        float spawnX = lastSpawnX + spacing;
        float spawnY = Random.Range(minY, maxY);

        go.transform.position = new Vector3(spawnX, spawnY, 0f);
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        // --- ریست یا بازسازی الماس‌ها مطابق با prefab مرجع ---
        // خروجی: اگر prefab مرجع دارای childهایی با کامپوننت Diamond باشد، 
        // اطمینان حاصل شود که این instance هم آن childها را (فعال) دارد.
        GameObject sourcePrefab = platformPrefabs[idx];

        // 1) پیدا کردن تمام Transform های دارای Diamond در prefab مرجع
        var prefabDiamondTransforms = sourcePrefab.GetComponentsInChildren<Diamond>(true);

        // 2) برای هر الماس مرجع، چک کن آیا در instance یک الماس با نام/پوزیشن مشابه وجود دارد
        foreach (var prefabDiamond in prefabDiamondTransforms)
        {
            // سعی می‌کنیم نزدیک‌ترین child را با نام یا موقعیت پیدا کنیم.
            // ساده‌ترین و مطمئن‌ترین راه اینه که اسم prefab child را نگاه کنیم:
            string childName = prefabDiamond.gameObject.name;

            // جستجو در instance برای وجود نام مشابه
            Transform existing = go.transform.Find(childName);
            if (existing != null)
            {
                // اگر وجود داره، فقط مطمئن شو فعال و ریست شده
                existing.gameObject.SetActive(true);
                var sr = existing.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = new Color(1f, 1f, 1f, 1f);
                var col = existing.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;
                // OnEnable در Diamond ریست رو انجام میده
            }
            else
            {
                // اگر وجود نداره (احتمالاً چون قبلاً Destroy شده)، یک کپی از prefab child بساز
                // و آن را به عنوان child پلتفرم فعلی اضافه کن، و local transform را حفظ کن.
                Transform prefabTrans = prefabDiamond.transform;
                GameObject newDiamond = Instantiate(prefabTrans.gameObject, go.transform);
                // قرار دادن دقیق مکان محلی و چرخش و مقیاس مطابق prefab
                newDiamond.transform.localPosition = prefabTrans.localPosition;
                newDiamond.transform.localRotation = prefabTrans.localRotation;
                newDiamond.transform.localScale = prefabTrans.localScale;
            }
        }
        // --- پایان بازسازی الماسها ---

        activeList.Add(go);
        lastSpawnX = spawnX;
    }
}
