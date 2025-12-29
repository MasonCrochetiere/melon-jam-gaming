using System.Collections;
using TMPro;
using UnityEngine;

public class SubtitleText : MonoBehaviour
{
    [SerializeField] float clearSubtitleTime = 8f;

    TMP_Text text;

    Coroutine subtitleCoroutine;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        text.text = "";
    }
    public void SetSubtitleText(string dialogue, Color color)
    {
        text.text = dialogue;
        text.color = color;

        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
        }
        subtitleCoroutine = StartCoroutine(ClearSubtitles(clearSubtitleTime));
    }

    IEnumerator ClearSubtitles(float delay)
    {
        yield return new WaitForSeconds(delay);

        text.text = "";
    }
}
