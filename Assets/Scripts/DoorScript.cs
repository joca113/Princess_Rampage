using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class DoorScript : MonoBehaviour
{
    public GameObject container;
    private Animator animator;

    [SerializeField] private TMP_Text finalTime;
    [SerializeField] private TMP_Text bestTime;

    private LevelTimer levelTimer;

    [Header("Audio")]
    public AudioClip winOption02;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        levelTimer = FindFirstObjectByType<LevelTimer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        StartCoroutine(DoorOpenSequence());
    }

    IEnumerator DoorOpenSequence()
    {
        audioSource.PlayOneShot(winOption02, 0.5f);
        yield return new WaitForSeconds(1.35f);
        animator.Play("Door_end");
        yield return StartCoroutine(EndPhase());
    }

    IEnumerator EndPhase()
    {
        yield return new WaitForSeconds(1f);

        levelTimer.StopAndSaveBestTime();

        Time.timeScale = 0;
        container.SetActive(true);
        
        finalTime.text = $"You completed the level in {levelTimer.GetFormattedCurrentTime()}";
        bestTime.text = $"Best time: {levelTimer.GetFormattedBestTime()}";
        levelTimer.gameObject.SetActive(false);
    }

    public void MenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
