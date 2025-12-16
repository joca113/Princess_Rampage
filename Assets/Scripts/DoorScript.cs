using UnityEngine;
using System.Collections;
using TMPro;
using System.Threading;

public class DoorScript : MonoBehaviour
{
    public GameObject container;
    private Animator animator;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text finalTime;
    private void OnTriggerEnter2D(Collider2D collision)

    {
        if (collision.gameObject.tag == "Player")
        {
            animator.Play("Door_end");
            StartCoroutine(EndPhase());
        }

    }

    IEnumerator EndPhase()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
        container.SetActive(true);
        finalTime.text = $"You completed the level in {timerText.text} minutes.";
        timerText.gameObject.SetActive(false);
    }

    public void MenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    } 
}
