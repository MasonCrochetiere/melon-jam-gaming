using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject jumpObject;
    [SerializeField] GameObject dashObject;
    [SerializeField] GameObject ballObject;
    [SerializeField] GameObject auraObject;

    private void Start()
    {
        jumpObject.SetActive(false);
        ballObject.SetActive(false);
        dashObject.SetActive(false);
        auraObject.SetActive(false);
    }

    public void UnlockItem(ItemList item)
    {
        switch (item)
        {
            case ItemList.Jump:
                jumpObject.SetActive(true);
                break;
            case ItemList.Dash:
                dashObject.SetActive(true);
                break;
            case ItemList.Ball:
                ballObject.SetActive(true);
                break;
            case ItemList.Aura:
                auraObject.SetActive(true);
                break;
        }

    }
}
