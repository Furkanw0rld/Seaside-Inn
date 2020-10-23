using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButcher : NPCInventorySystem
{
    void Start()
    {
        shopInventory = ShopInventoryManager.Instance;
        playerInventory = PlayerInventory.Instance;
        gameTimeManager = GameTimeManager.Instance;
    }

    public override IEnumerator ProduceItems()
    {
        for(int i = 0; i < itemsToProduce.Length; i++)
        {
            int index = FindItemInInventory(itemsToProduce[i]);

            if(index >= 0 && inventorySlots[index].amount <= 6) //Found the item and we can add to it
            {
                inventorySlots[index].amount += Random.Range(0, 5);
            }
            else if(index < 0) //The character doesn't have the items, add to a new slot
            {
                inventorySlots.Add(new InventorySlotItem(itemsToProduce[i], Random.Range(0, 5)));
            }

            yield return null;
        }
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
}
