using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference levelMusic { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("Player")]
    [field: SerializeField] public EventReference footstep { get; private set; }
    [field: SerializeField] public EventReference itemCollect { get; private set; }
    [field: SerializeField] public EventReference jump { get; private set; }
    [field: SerializeField] public EventReference maskCollect { get; private set; }
    [field: SerializeField] public EventReference playerDeath { get; private set; }
    [field: SerializeField] public EventReference playerHurt { get; private set; }

    [field: Header("Level")]
    [field: SerializeField] public EventReference timeAlert { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference buttonClick { get; private set; }

    [field: Header("Dialogue")]



    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events script in the scene.");
        }
        instance = this;
    }
}
