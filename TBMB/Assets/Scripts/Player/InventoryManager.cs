using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemList { Jump, Dash, Ball, Aura }

public class InventoryManager : MonoBehaviour
{
    public Dictionary<ItemList, bool> inventory = new Dictionary<ItemList, bool>();

    [SerializeField] UnityEvent<ItemList> unlockItemEvent;

    private void Start()
    {
        inventory.Add(ItemList.Jump, false); 
        inventory.Add(ItemList.Dash, false);
        inventory.Add(ItemList.Ball, false);
        inventory.Add(ItemList.Aura, false);
    }

    public void UnlockItem(ItemList item)
    {
        inventory[item] = true;

        unlockItemEvent.Invoke(item);
    }

    public bool CheckItem(ItemList item)
    {
        return inventory[item];
    }

}
