using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public AudioSource GameoverAudio;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(GameOverSequence());
        }
    }

    private System.Collections.IEnumerator GameOverSequence()
    {
        // پخش صدا
        if (GameoverAudio != null)
            GameoverAudio.Play();

        // صبر تا وقتی که صدا تمام شود
        if (GameoverAudio != null)
            yield return new WaitForSeconds(GameoverAudio.clip.length);
        else
            yield return new WaitForSeconds(0.5f); // اگر اشتباهی صدا نداشت

        // لود صحنه منو
        GameManager.Instance.LoadSceneByName("MainMenu");
    }
}