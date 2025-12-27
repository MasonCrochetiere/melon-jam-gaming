using System.Security.Cryptography;
using UnityEngine;

public class DashPoint : MonoBehaviour
{
    [SerializeField] GameObject dashAngleViewer;
    [SerializeField] Animator dashAngleAnimator;

    bool angleLocked = false;
    private void Start()
    {
        if (dashAngleViewer == null)
            return;
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
            dashAngleAnimator.SetBool("PointActive", true);
        }
        else
        {
            dashAngleAnimator.SetBool("PointActive", false);
            angleLocked = false;
        }
    }

    public void LockAngle()
    {
        angleLocked = true;
    }

    public void UpdateDashAngle(float angle)
    {
        if (dashAngleViewer == null || angleLocked)
            return;

        dashAngleViewer.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
