using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public GameObject container;
    private Animator animator;
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
