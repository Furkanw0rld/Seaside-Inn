using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButcher : NPCInventorySystem
{

#pragma warning disable 0649
    [Header("Production Items")]
    [SerializeField] private Food_Item[] itemsToProduce;
#pragma warning restore 0649

    // Start is called before the first frame update
    void Start()
    {
        shopInventory = ShopInventoryManager.Instance;
        playerInventory = PlayerInventory.Instance;
        gameTimeManager = GameTimeManager.Instance;
    }

    public IEnumerator ProduceItems()
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

    private int FindItemInInventory(Food_Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item.name == item.name)
            {
                return i;
            }
        }

        return -1;
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
