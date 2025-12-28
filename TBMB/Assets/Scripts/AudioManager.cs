using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]

    public float masterVolume = 1;
    [Range(0, 1)]

    public float musicVolume = 1;
    [Range(0, 1)]

    public float SFXVolume = 1;
    [Range(0, 1)]

    public float dialogueVolume = 1;
    [Range(0, 1)]

    private Bus masterBus;

    public Bus musicBus;

    private Bus SFXBus;

    private Bus dialogueBus;

    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager script in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.ambience);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);
        dialogueBus.setVolume(dialogueVolume);
    }

    public void PlayeOneShot2D(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void UpdateMusicParameter(MusicEnum musicPhase)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("LevelPhase", ((float)musicPhase));
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // Stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // Stop all of the event emitters
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}