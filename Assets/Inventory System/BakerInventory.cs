using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BakerInventory : MonoBehaviour
{
    public List<InventorySlotItem> inventorySlots = new List<InventorySlotItem>();

    public void OpenShop()
    {
        ShopInventoryManager.Instance.ClearContentWindow();
        foreach (InventorySlotItem slot in inventorySlots)
        {
            if(slot.amount > 0)
            {
                NPC_ItemSlotInformationData slotInformation = ShopInventoryManager.Instance.AddItem(slot);
                slotInformation.purchaseButton.onClick.RemoveAllListeners();
                slotInformation.purchaseButton.onClick.AddListener(delegate{ BuyItem(slot, slotInformation.purchaseButton, slotInformation); });

                if(PlayerInventory.Instance.GetCoins() < slot.item.itemPrice) // Player doesn't have enough coin for this item, disable it.
                {
                    slotInformation.purchaseButton.interactable = false;
                }
            }
        }

        ShopInventoryManager.Instance.shopWindow.SetActive(true);
    }

    public void CloseShop()
    {
        ShopInventoryManager.Instance.shopWindow.SetActive(false);
    }

    public void BuyItem(InventorySlotItem slot, Button purchaseButton, NPC_ItemSlotInformationData slotInformation)
    {
        if(PlayerInventory.Instance.GetCoins() >= slot.item.itemPrice)
        {
            if(PlayerInventory.Instance.BuyItem(slot.item, slot.item.itemPrice))
            {
                slot.RemoveAmount(1);
                slotInformation.itemAmount.text = slot.amount.ToString();

                if (slot.amount == 0)
                {
                    ShopInventoryManager.Instance.RemoveItem(slotInformation);
                }

                ShopInventoryManager.Instance.UpdateContentWindowItems();
            }
        }
    }

}

