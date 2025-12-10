using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    public void OnPause(InputAction.CallbackContext context)
    {

       if (!context.performed) return;

        bool paused = container.activeSelf;

        container.SetActive(!paused);
        Time.timeScale = paused ? 1 : 0;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
