using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    [SerializeField] ItemList itemType;
    [SerializeField] UnityEvent onPickup;

    bool picked = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !picked)
        {
            InventoryManager inv = collision.gameObject.GetComponent<InventoryManager>();
            inv.UnlockItem(itemType);

            picked = true;

            onPickup.Invoke();
        }
    }
}
