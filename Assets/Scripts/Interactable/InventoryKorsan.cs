using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryKorsan : NPCInventorySystem
{
    // Start is called before the first frame update
    void Start()
    {
        shopInventory = ShopInventoryManager.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    public override void OpenShop()
    {
        shopInventory.ClearContentWindow();
        foreach (InventorySlotItem slot in inventorySlots)
        {
            if (slot.amount > 0)
            {
                NPC_ItemSlotInformationData slotInformation = shopInventory.AddItem(slot);
                slotInformation.purchaseButton.onClick.RemoveAllListeners();
                slotInformation.purchaseButton.onClick.AddListener(delegate { BuyItem(slot, slotInformation.purchaseButton, slotInformation); });

                if (playerInventory.GetCoins() < slot.item.itemPrice) // Player doesn't have enough coin for this item, disable it.
                {
                    slotInformation.purchaseButton.interactable = false;
                }
            }
        }

        shopInventory.shopWindow.SetActive(true);
    }

    public override void CloseShop()
    {
        shopInventory.shopWindow.SetActive(false);
    }

    public override void BuyItem(InventorySlotItem slot, Button purchaseButton, NPC_ItemSlotInformationData slotInformation)
    {
        if (playerInventory.GetCoins() >= slot.item.itemPrice)
        {
            if (playerInventory.BuyItem(slot.item, slot.item.itemPrice))
            {
                slot.RemoveAmount(1);
                slotInformation.itemAmount.text = slot.amount.ToString();

                if (slot.amount == 0)
                {
                    shopInventory.RemoveItem(slotInformation);
                }

                shopInventory.UpdateContentWindowItems();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
