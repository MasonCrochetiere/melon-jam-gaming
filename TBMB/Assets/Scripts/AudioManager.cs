using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in scene.");
        }
        instance = this;
    }

    public void PlayeOneShot2D(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }
}
