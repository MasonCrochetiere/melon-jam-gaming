using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] ItemList itemType;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager inv = collision.gameObject.GetComponent<InventoryManager>();
            inv.UnlockItem(itemType);

            Destroy(gameObject);
        }
    }
}
