using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
         Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
