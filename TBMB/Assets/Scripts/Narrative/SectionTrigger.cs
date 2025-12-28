using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField] int section;
    [SerializeField] int linesInSection;

    bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            triggered = true;
            NarrativeManager.instance.NewSection(section, linesInSection);
        }
    }
}
