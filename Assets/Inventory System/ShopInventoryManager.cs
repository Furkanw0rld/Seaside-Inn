using System.Collections.Generic;
using UnityEngine;

public class ShopInventoryManager : MonoBehaviour
{
    public static ShopInventoryManager Instance;
    [Tooltip("Contains item display data.")] public GameObject inventorySlot;
    [Tooltip("The area where the content will be displayed.")] public GameObject contentWindow;
    [Tooltip("Shop Inventory UI")] public GameObject shopWindow;
    private List<NPC_ItemSlotInformationData> slots = new List<NPC_ItemSlotInformationData>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public NPC_ItemSlotInformationData AddItem(InventorySlotItem slot)
    {
        GameObject createdSlot = Instantiate(inventorySlot, contentWindow.transform);
        NPC_ItemSlotInformationData itemData = createdSlot.GetComponent<NPC_ItemSlotInformationData>();
        slots.Add(itemData);

        itemData.itemName.text = slot.item.GetLocalizedName();
        itemData.itemName.color = slot.item.GetQualityColor();
        itemData.itemIcon.sprite = slot.item.icon;
        itemData.itemDescription.text = slot.item.GetLocalizedDescription();
        itemData.itemPrice.text = slot.item.itemPrice.ToString();
        itemData.itemQuality.text = slot.item.GetQuality().ToUpper();
        itemData.itemQuality.color = slot.item.GetQualityColor();
        itemData.itemAmount.text = slot.amount.ToString();
        return itemData;
    }

    public void RemoveItem(NPC_ItemSlotInformationData slotInformation)
    {
        slots.Remove(slotInformation);
        Destroy(slotInformation.gameObject);
    }

    public void UpdateContentWindowItems()
    {
        if(slots.Count > 0)
        {
            for(int i = 0; i < slots.Count; i++)
            {
                if (PlayerInventory.Instance.GetCoins() < float.Parse(slots[i].itemPrice.text))
                {
                    slots[i].purchaseButton.interactable = false;
                }
            }
        }
    }

    public void ClearContentWindow()
    {
        if(slots.Count > 0)
        {
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                Destroy(slots[i].gameObject);
                slots.RemoveAt(i);
                
            }
        }
    }
}
