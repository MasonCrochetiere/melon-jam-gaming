using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class NativeAudioSlider : MonoBehaviour
{
    public AudioMixer dialogueMixer;
    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    public void OnSliderValueChanged()
    {
        float sliderValue = volumeSlider.value;
        // Conver linear to log
        float volumeDB = sliderValue > 0 ? 20f * Mathf.Log10(sliderValue) : -80f;

        Debug.Log($"Slider Value: {sliderValue}, Volume dB: {volumeDB}");
        dialogueMixer.SetFloat("DialogueVol", volumeDB);
    }
}