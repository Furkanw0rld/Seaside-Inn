using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class NPCInventorySystem : MonoBehaviour
{
    [Header("Active Inventory")]
    public List<InventorySlotItem> inventorySlots = new List<InventorySlotItem>();

#pragma warning disable 0649
    [Header("Production Items")]
    [SerializeField] protected Item[] itemsToProduce;
#pragma warning restore 0649

    protected ShopInventoryManager shopInventory;
    protected PlayerInventory playerInventory;
    protected GameTimeManager gameTimeManager;

    public abstract void OpenShop();
    public abstract void CloseShop();
    public abstract void BuyItem(InventorySlotItem slot, Button purchaseButton, NPC_ItemSlotInformationData slotInformation);

    public virtual IEnumerator ProduceItems()
    {
        yield return null;
    }

    protected int FindItemInInventory(Item item)
    {
        if(inventorySlots.Count < 1)
        {
            return -1;
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item.name == item.name)
            {
                return i;
            }
        }

        return -1;
    }
}
