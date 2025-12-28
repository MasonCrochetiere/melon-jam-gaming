using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager instance;

    [SerializeField] float timeBetweenDillyDallyLines = 20f;
    [SerializeField] List<float> sectionFastTimes;

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
        // Play the line of index currentLine in section currentSection
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

                break;
            case ItemList.Dash:

                break;
            case ItemList.Ball:

                break;
            case ItemList.Aura:
                // @ Thomas -- Need cutscene integration here
                break;
        }
    }

    public void NewSection(int section, int _linesInSection)
    {
        currentSection = section;
        linesInSection = _linesInSection;
        currentLine = 0;

        if (Time.time - lastSectionStartTime <= sectionFastTimes[section]) // TIME CHECK
        {
            // PLAY FAST LINE

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
