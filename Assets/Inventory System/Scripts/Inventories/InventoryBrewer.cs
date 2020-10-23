using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBrewer : NPCInventorySystem
{
    void Start()
    {
        shopInventory = ShopInventoryManager.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    public override IEnumerator ProduceItems()
    {
        for(int i = 0; i < itemsToProduce.Length; i++)
        {
            int index = FindItemInInventory(itemsToProduce[i]);

            if (index >= 0 && inventorySlots[index].amount <= 20) 
            {
                inventorySlots[index].amount += Random.Range(0, 4) * 5; //Add between 0 - 15, at increments of 5. (0 - 15)
            }
            else if (index < 0)
            {
                inventorySlots.Add(new InventorySlotItem(itemsToProduce[i], Random.Range(1, 5) * 5)); //Create a new one, at an increment of 5. (5 - 20)
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

        if (slot.item.itemType == ItemType.Drink)
        {
            int index = playerInventory.FindAnotherItemInInnWithSpace(0, slot.item);
            if (index < 0)
            {
                return;
            }

            int spaceRemaining = playerInventory.innInventory[index].InventorySpaceRemaining();

            if (slot.amount < spaceRemaining && slot.amount < 5)
            {
                if (playerInventory.GetCoins() >= slot.item.itemPrice)
                {
                    if (playerInventory.BuyItem(slot.item, slot.item.itemPrice, slot.amount))
                    {
                        slot.RemoveAmount(slot.amount);
                        slotInformation.itemAmount.text = slot.amount.ToString();

                        if (slot.amount <= 0)
                        {
                            shopInventory.RemoveItem(slotInformation);
                        }
                        shopInventory.UpdateContentWindowItems();
                    }
                }
            }
            else
            {
                if (playerInventory.GetCoins() >= slot.item.itemPrice)
                {
                    if (playerInventory.BuyItem(slot.item, slot.item.itemPrice))
                    {
                        if (spaceRemaining < 5)
                        {
                            slot.RemoveAmount(spaceRemaining);
                        }
                        else
                        {
                            slot.RemoveAmount(5);
                        }

                        slotInformation.itemAmount.text = slot.amount.ToString();

                        if (slot.amount <= 0)
                        {
                            shopInventory.RemoveItem(slotInformation);
                        }

                        shopInventory.UpdateContentWindowItems();
                    }
                }
            }
        }
        else
        {
            if (playerInventory.GetCoins() >= slot.item.itemPrice)
            {
                if (playerInventory.BuyItem(slot.item, slot.item.itemPrice))
                {
                    slot.RemoveAmount(1);

                    slotInformation.itemAmount.text = slot.amount.ToString();

                    if (slot.amount <= 0)
                    {
                        shopInventory.RemoveItem(slotInformation);
                    }

                    shopInventory.UpdateContentWindowItems();
                }
            }
        }

    }
}
