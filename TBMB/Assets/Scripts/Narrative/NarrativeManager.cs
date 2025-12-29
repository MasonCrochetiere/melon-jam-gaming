using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoiceLineType { Tutorial, Dilldally, Fast }

[System.Serializable]
public class VoiceLine
{
    [SerializeField] public VoiceLineType type;
    [SerializeField] public int section;
    [SerializeField] public int index;

    [SerializeField] public AudioClip audioClip;
    [SerializeField] string text; 
}

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance;

    [SerializeField] float timeBetweenDillyDallyLines = 20f;
    [SerializeField] List<float> sectionFastTimes;

    [SerializeField] AudioSource narrativeAudio;

    [SerializeField] List<VoiceLine> voiceLines;

    int currentSection;
    int currentLine;
    int linesInSection;

    Coroutine dillyDallyCoroutine;

    float lastSectionStartTime;

    private void Start()
    {
        instance = this;

        NewSection(0, 1);

        // Section 0 = start of game, opening cutscene
        // Section 1 = Jump section
        // Section 2 = Dash section
        // Section 3 = Ball section
        // Section 4 = All mechanics / Dinosaur section
        // Section 5 = cutscene one can we even use this stuff?
    }

    void DillyDallyLine()
    {
        Debug.Log("PLAYING DILLYDALLY LINE #" + currentLine + " IN SECTION #" + currentSection);
        PlayVoiceLine(VoiceLineType.Dilldally, currentSection, currentLine);
        currentLine++;

        // should we have the lines loop? this functionality does not include that
        // I also did not implement randomized behavior, I thought it was unnecessary.

        if (currentLine < linesInSection)
        {
            RunCoroutine();
        }
    }

    void RunCoroutine()
    {
        if (dillyDallyCoroutine != null)
        {
            StopCoroutine(dillyDallyCoroutine);
        }
        dillyDallyCoroutine = StartCoroutine(WaitForDillyDallyLine(timeBetweenDillyDallyLines));
    }

    public void TutorialLine(ItemList item)
    {
        Debug.Log("PLAYING TUTORIAL LINE FOR " + item.ToString());

        switch (item)
        {
            case ItemList.Jump:
                PlayVoiceLine(VoiceLineType.Tutorial, 0, 0);
                break;
            case ItemList.Dash:
                PlayVoiceLine(VoiceLineType.Tutorial, 1, 0);
                break;
            case ItemList.Ball:
                PlayVoiceLine(VoiceLineType.Tutorial, 2, 0);
                break;
            case ItemList.Aura:
                Invoke("AuraLine", 7f);
                break;
        }
    }

    void AuraLine()
    {
        PlayVoiceLine(VoiceLineType.Tutorial, 3, 0);
    }

    public void PlayVoiceLine(VoiceLineType type, int section, int index)
    {
        foreach (VoiceLine line in voiceLines)
        {
            if (line.type == type && line.section == section && line.index == index)
            {
                narrativeAudio.Stop();
                narrativeAudio.PlayOneShot(line.audioClip);

                Debug.LogWarning("WE FOUND LINE TO PLAY IT'S TYPE " + type.ToString() + " SECTION " + section.ToString() + " INDEX " + index.ToString());

                return;
            }
        }

        Debug.LogError("LINE NOT FOUND!");

    }

    public void NewSection(int section, int _linesInSection)
    {
        currentSection = section;
        linesInSection = _linesInSection;
        currentLine = 0;

        if (Time.time - lastSectionStartTime <= sectionFastTimes[section] && section != 0) // TIME CHECK
        {
            // PLAY FAST LINE
            PlayVoiceLine(VoiceLineType.Fast, currentSection, 0);
            Debug.Log("PLAYING FAST LINE FOR " + section);

            RunCoroutine();
        }
        else
        {
            DillyDallyLine();
        }

        lastSectionStartTime = Time.time;
    }

    IEnumerator WaitForDillyDallyLine(float delay)
    {
        yield return new WaitForSeconds(delay);

        DillyDallyLine();
    }
}
