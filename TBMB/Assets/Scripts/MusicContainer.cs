using FMOD.Studio;
using System.Reflection;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MusicContainer : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private MusicEnum musicEnum = MusicEnum.LevelOutro;
    public EventInstance music;

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
}
