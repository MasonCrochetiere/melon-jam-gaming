using TMPro;
using UnityEngine;

public class SubtitleText : MonoBehaviour
{
    [SerializeField] float clearSubtitleTime = 8f;

    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        text.text = "";
    }
    public void SetSubtitleText(string dialogue, Color color)
    {
        text.text = dialogue;
        text.color = color;

        Invoke("ClearSubtitle", clearSubtitleTime);
    }

    void ClearSubtitle()
    {
        text.text = "";
    }
}
