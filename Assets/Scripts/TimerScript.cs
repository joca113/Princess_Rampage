using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float elapsed;
    private bool running = true;

    private const string BestTimeKey = "BestTime_Level1"; // change per level

    void Update()
    {
        if (!running) return;

        elapsed += Time.deltaTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        timerText.text = FormatTime(elapsed);
    }

    public void StopAndSaveBestTime()
    {
        running = false;

        float best = PlayerPrefs.GetFloat(BestTimeKey, float.PositiveInfinity);

        if (elapsed < best)
        {
            PlayerPrefs.SetFloat(BestTimeKey, elapsed);
            PlayerPrefs.Save();
        }
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat(BestTimeKey, float.PositiveInfinity);
    }

    public string GetFormattedCurrentTime()
    {
        return FormatTime(elapsed);
    }

    public string GetFormattedBestTime()
    {
        float best = GetBestTime();
        return float.IsInfinity(best) ? "--:--" : FormatTime(best);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    [ContextMenu("Reset Best Time")]
    public void ResetBestTime() // usem para resetar o recorde, se quiserem (GameObject->LevelTimer->3 pontinhos->Reset Best Time).
    {
        PlayerPrefs.DeleteKey(BestTimeKey);
        PlayerPrefs.Save();
        Debug.Log("Best time reset!");
    }
}
