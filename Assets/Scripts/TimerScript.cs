using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float elapsed;
    private bool running = true;

    void Update()
    {
        if (!running) return;

        elapsed += Time.deltaTime; // counts game time (stops if timeScale = 0)
        UpdateUI();
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        running = false;
    }

    public void ResetTimer()
    {
        elapsed = 0f;
        UpdateUI();
    }
}
