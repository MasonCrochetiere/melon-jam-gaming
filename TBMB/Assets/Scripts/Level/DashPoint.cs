using UnityEngine;

public class DashPoint : MonoBehaviour
{
    [SerializeField] GameObject dashAngleViewer;
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

    public void UpdateDashAngle(GameObject player)
    {

    }
}
