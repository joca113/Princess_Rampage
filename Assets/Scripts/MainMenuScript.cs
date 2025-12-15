using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public AudioClip clickAudio;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
   

    public void StartGame()
    {
        audioSource.PlayOneShot(clickAudio);
        SceneManager.LoadScene("GameScene");
         Time.timeScale = 1;
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(clickAudio);
        Application.Quit();
    }
}
