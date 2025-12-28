using Unity.VisualScripting;
using UnityEngine;

public class KillPlayerTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // death sound probably goes in here rather than anywhere else
            // make sure the sound can only play one at a time since otherwise it'll spam unfortunately
            collision.gameObject.GetComponent<PlayerController>().KillPlayer();
        }
    }
}
