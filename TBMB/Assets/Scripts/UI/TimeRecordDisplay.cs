using TMPro;
using UnityEngine;

public class TimeRecordDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text recordText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float time = SceneTimeTracker.instance.GetElapsedTime();
        float bestTime = SceneTimeTracker.instance.GetBestTime();

        timeText.text = "Time: " + FormatTime(time);
        recordText.text = "Best Time: " + FormatTime(bestTime);

        if (time == bestTime)
        {
            timeText.color = Color.yellow;
            recordText.color = Color.yellow;
        }
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

}
