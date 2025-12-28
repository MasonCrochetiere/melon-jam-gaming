using FMOD.Studio;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MusicContainer : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private MusicEnum musicEnum = MusicEnum.LevelOutro;
    public EventInstance music;

    [Header("Cutscene Dim Amount")]
    [Range(0, 1)]
    public float musicDim = 0.5f;

    public float dimDuration = 8f;

    private float originalVolume;

    private bool isDimmed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music = AudioManager.instance.CreateInstance(FMODEvents.instance.levelMusic);
        music.start();
    }

    public void TriggerMusicChange()
    {
        AudioManager.instance.UpdateMusicParameter(musicEnum);
    }

    public void DimMusic()
    {
        if (!isDimmed)
        {
            AudioManager.instance.musicBus.getVolume(out originalVolume);
            AudioManager.instance.musicVolume = originalVolume * musicDim;
            isDimmed = true;

            StartCoroutine(RestoreMusicAfterDelay());
        }
    }

    private IEnumerator RestoreMusicAfterDelay()
    {
        yield return new WaitForSeconds(dimDuration);
        RestoreMusic();
    }

    public void RestoreMusic()
    {
        if (isDimmed)
        {
            AudioManager.instance.musicVolume = originalVolume;
            isDimmed = false;
        }
    }
}
