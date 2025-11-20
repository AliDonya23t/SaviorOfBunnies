using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class CutsceneController : MonoBehaviour
{
    [Header("Images (order shown)")]
    public List<Sprite> images;

    [Header("UI References")]
    public Image displayImage;        // assign in inspector
    CanvasGroup canvasGroup;

    [Header("Audio")]
    public AudioClip backgroundMusic;            // looped background music (optional)
    public List<AudioClip> imageSounds;          // optional: one clip per image (can be empty or shorter)
    AudioSource audioSource;

    [Header("Timings")]
    public float fadeDuration = 0.8f;
    public float displayDuration = 2.0f;

    [Header("Advance & Skip")]
    public bool allowClickAdvance = true;
    public bool allowSkipKey = true;
    public KeyCode skipKey = KeyCode.Space;
    public KeyCode nextImageKey = KeyCode.Return;

    [Header("Ken Burns (pan/zoom)")]
    public bool enableKenBurns = true;
    public Vector2 kenStartScale = new Vector2(1f, 1f);
    public Vector2 kenEndScale = new Vector2(1.08f, 1.08f);
    public Vector2 kenStartOffset = Vector2.zero;
    public Vector2 kenEndOffset = new Vector2(0f, 30f);

    [Header("After cutscene")]
    public string nextSceneName = "";
    public bool deactivateWhenDone = false;

    [Header("Show only first time")]
    public bool onlyShowFirstTime = true;
    public string playerPrefKey = "HasSeenCutscene_v1"; // change key if you edit cutscene to force re-show

    Coroutine playRoutine;
    bool isSkipping = false;
    bool isPlaying = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        if (displayImage == null)
        {
            Debug.LogError("CutsceneController: assign displayImage in inspector.");
            enabled = false;
            return;
        }

        // prepare audio
        audioSource.playOnAwake = false;
        audioSource.loop = false; // we'll control loop if backgroundMusic exists
        canvasGroup.alpha = 0f;

        // ensure UI fits parent
        FitImageToScreen(displayImage);
    }

    void OnEnable()
    {
        // if only show once and flag is set, skip cutscene
        if (onlyShowFirstTime && PlayerPrefs.GetInt(playerPrefKey, 0) == 1)
        {
            // directly go to next scene or disable
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else if (deactivateWhenDone)
            {
                gameObject.SetActive(false);
            }
            else
            {
                canvasGroup.alpha = 0f;
            }
            return;
        }

        if (images == null || images.Count == 0)
        {
            Debug.LogWarning("CutsceneController: no images assigned.");
            gameObject.SetActive(false);
            return;
        }

        // start background music if provided
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        playRoutine = StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        if (!isPlaying) return;

        if (allowSkipKey && Input.GetKeyDown(skipKey))
        {
            SkipAll();
        }
        if (allowClickAdvance && Input.GetMouseButtonDown(0))
        {
            AdvanceOne();
        }
        if (Input.GetKeyDown(nextImageKey))
        {
            AdvanceOne();
        }
    }

    IEnumerator PlayCutscene()
    {
        isPlaying = true;

        for (int i = 0; i < images.Count; i++)
        {
            if (isSkipping) break;

            displayImage.sprite = images[i];
            FitImageToScreen(displayImage);

            // reset transform for ken burns
            RectTransform rt = displayImage.rectTransform;
            rt.localScale = new Vector3(kenStartScale.x, kenStartScale.y, 1f);
            rt.anchoredPosition = kenStartOffset;

            // play sound for this image (if available)
            if (imageSounds != null && i < imageSounds.Count && imageSounds[i] != null)
            {
                audioSource.PlayOneShot(imageSounds[i]);
            }

            // fade in
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            // visible + ken-burns
            float elapsed = 0f;
            float visibleTime = displayDuration;
            while (elapsed < visibleTime)
            {
                if (isSkipping) break;
                float t = elapsed / visibleTime;
                if (enableKenBurns)
                {
                    float sX = Mathf.Lerp(kenStartScale.x, kenEndScale.x, t);
                    float sY = Mathf.Lerp(kenStartScale.y, kenEndScale.y, t);
                    rt.localScale = new Vector3(sX, sY, 1f);
                    rt.anchoredPosition = Vector2.Lerp(kenStartOffset, kenEndOffset, t);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (isSkipping) break;

            // fade out
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        }

        isPlaying = false;
        // mark as seen
        if (onlyShowFirstTime)
        {
            PlayerPrefs.SetInt(playerPrefKey, 1);
            PlayerPrefs.Save();
        }
        EndCutscene();
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < duration)
        {
            if (isSkipping)
            {
                canvasGroup.alpha = to;
                yield break;
            }
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    void AdvanceOne()
    {
        if (!isPlaying) return;

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }
        playRoutine = StartCoroutine(FastAdvanceRoutine());
    }

    IEnumerator FastAdvanceRoutine()
    {
        yield return StartCoroutine(Fade(canvasGroup.alpha, 0f, Mathf.Min(0.25f, fadeDuration)));
        int curIndex = images.IndexOf(displayImage.sprite);
        int nextIndex = curIndex + 1;
        if (nextIndex < images.Count)
        {
            playRoutine = StartCoroutine(PlayFromIndex(nextIndex));
            yield break;
        }
        else
        {
            isPlaying = false;
            // mark as seen
            if (onlyShowFirstTime)
            {
                PlayerPrefs.SetInt(playerPrefKey, 1);
                PlayerPrefs.Save();
            }
            EndCutscene();
            yield break;
        }
    }

    IEnumerator PlayFromIndex(int startIndex)
    {
        for (int i = startIndex; i < images.Count; i++)
        {
            if (isSkipping) break;
            displayImage.sprite = images[i];
            FitImageToScreen(displayImage);

            RectTransform rt = displayImage.rectTransform;
            rt.localScale = new Vector3(kenStartScale.x, kenStartScale.y, 1f);
            rt.anchoredPosition = kenStartOffset;

            // play sound for this image (if available)
            if (imageSounds != null && i < imageSounds.Count && imageSounds[i] != null)
            {
                audioSource.PlayOneShot(imageSounds[i]);
            }

            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            float elapsed = 0f;
            float visibleTime = displayDuration;
            while (elapsed < visibleTime)
            {
                if (isSkipping) break;
                float t = elapsed / visibleTime;
                if (enableKenBurns)
                {
                    float sX = Mathf.Lerp(kenStartScale.x, kenEndScale.x, t);
                    float sY = Mathf.Lerp(kenStartScale.y, kenEndScale.y, t);
                    rt.localScale = new Vector3(sX, sY, 1f);
                    rt.anchoredPosition = Vector2.Lerp(kenStartOffset, kenEndOffset, t);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (isSkipping) break;

            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        }

        isPlaying = false;
        if (onlyShowFirstTime)
        {
            PlayerPrefs.SetInt(playerPrefKey, 1);
            PlayerPrefs.Save();
        }
        EndCutscene();
    }

    void SkipAll()
    {
        isSkipping = true;
        if (playRoutine != null) StopCoroutine(playRoutine);
        // stop audio
        if (audioSource != null) audioSource.Stop();
        canvasGroup.alpha = 0f;
        isPlaying = false;
        // mark as seen
        if (onlyShowFirstTime)
        {
            PlayerPrefs.SetInt(playerPrefKey, 1);
            PlayerPrefs.Save();
        }
        EndCutscene();
    }

    void EndCutscene()
    {
        // stop background music if playing
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (deactivateWhenDone)
        {
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    // make sure UI fills parent and keeps aspect ratio
    void FitImageToScreen(Image img)
    {
        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        img.preserveAspect = true;
    }
}
