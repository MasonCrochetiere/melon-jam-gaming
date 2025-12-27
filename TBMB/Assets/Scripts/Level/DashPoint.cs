using UnityEngine;

public class DashPoint : MonoBehaviour
{
    [SerializeField] GameObject dashAngleViewer;

    private void Start()
    {
        if (dashAngleViewer == null)
            return;

        dashAngleViewer.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().SetDashPoint(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RemoveDashPoint(this);
        }
    }

    public void ShowDashAngle(bool value)
    {
        if (dashAngleViewer == null)
            return;

        if (value)
        {
            dashAngleViewer.SetActive(true);
        }
        else
        {
            dashAngleViewer.SetActive(false);
        }
    }

    public void UpdateDashAngle(float angle)
    {
        if (dashAngleViewer == null)
            return;

        dashAngleViewer.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
