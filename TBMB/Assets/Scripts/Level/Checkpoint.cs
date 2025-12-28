using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().SetRespawnPoint(respawnPoint.position);
        }
    }
}
