using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] float timerDuration;
    [SerializeField] float dangerTime = 30f;

    TMP_Text timerText;

    public float timerStartTime;
    bool dangerTriggered = false;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
        //StartTimer();

    }

    private void Update()
    {
        float timeLeft =  (timerDuration - (Time.time - timerStartTime));
        
        timerText.text = FormatTime(timeLeft);

        if (timeLeft < dangerTime && !dangerTriggered)
        {
            animator.SetTrigger("TimerDanger");
            dangerTriggered = true;
        }
    }

    public void StartTimer()
    {
        timerStartTime = Time.time;
        animator.SetTrigger("StartTimer");
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
