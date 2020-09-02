using System.Collections.Generic;
using UnityEngine;

public class ShopInventoryManager : MonoBehaviour
{
    public static ShopInventoryManager Instance;
    [Tooltip("Contains item display data.")] public GameObject inventorySlot;
    [Tooltip("The area where the content will be displayed.")] public GameObject contentWindow;
    [Tooltip("Shop Inventory UI")] public GameObject shopWindow;
    private List<NPC_ItemSlotInformationData> slots = new List<NPC_ItemSlotInformationData>();
    private PlayerInventory playerInventory;

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

    private void Start()
    {
        playerInventory = PlayerInventory.Instance;
    }

    public NPC_ItemSlotInformationData AddItem(InventorySlotItem slot)
    {
        GameObject createdSlot = Instantiate(inventorySlot, contentWindow.transform);
        NPC_ItemSlotInformationData itemData = createdSlot.GetComponent<NPC_ItemSlotInformationData>();
        slots.Add(itemData);

        itemData.item = slot.item;
        itemData.itemName.text = slot.item.GetLocalizedName();
        itemData.itemName.color = slot.item.GetQualityColor();
        itemData.itemIcon.sprite = slot.item.icon;
        itemData.itemDescription.text = slot.item.GetLocalizedDescription();
        itemData.itemPriceText.text = slot.item.itemPrice.ToString();
        itemData.itemQuality.text = slot.item.GetQuality().ToUpper();
        itemData.itemQuality.color = slot.item.GetQualityColor();
        itemData.itemAmount.text = slot.amount.ToString();

        if (slot.item.itemType == ItemType.Drink) //Special case for drinks
        {
            int index = playerInventory.FindAnotherItemInInnWithSpace(0, slot.item);
            if (index < 0)
            {
                itemData.purchaseButton.interactable = false;
                itemData.itemName.text = slot.item.name;
                itemData.itemPriceText.text = slot.item.itemPrice.ToString();
            }
            else
            {
                int spaceRemaining = playerInventory.innInventory[index].InventorySpaceRemaining();
                if (spaceRemaining < 5)
                {
                    itemData.itemName.text += " (x" + spaceRemaining + ")";
                    itemData.itemPriceText.text = (slot.item.itemPrice * spaceRemaining).ToString();

                }
                else
                {
                    itemData.itemName.text += " (x5)";
                    itemData.itemPriceText.text = (slot.item.itemPrice * 5f).ToString();
                }

                if (slot.amount < 5)
                {
                    itemData.itemName.text = slot.item.name + " (x" + slot.amount + ")";
                    itemData.itemPriceText.text = (slot.item.itemPrice * slot.amount).ToString();
                }
            }

        }

        return itemData;
    }

    public void RemoveItem(NPC_ItemSlotInformationData slotInformation)
    {
        slots.Remove(slotInformation);
        Destroy(slotInformation.gameObject);
    }

    public void UpdateContentWindowItems()
    {
        if (slots.Count > 0)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item.itemType == ItemType.Drink)
                {
                    if (playerInventory.GetCoins() < slots[i].item.itemPrice)
                    {
                        slots[i].purchaseButton.interactable = false;
                        slots[i].itemName.text = slots[i].item.name;
                        slots[i].itemPriceText.text = slots[i].item.itemPrice.ToString();
                        continue;
                    }

                    int index = playerInventory.FindAnotherItemInInnWithSpace(0, slots[i].item);

                    if (index < 0)
                    {
                        slots[i].purchaseButton.interactable = false;
                        slots[i].itemName.text = slots[i].item.name;
                        slots[i].itemPriceText.text = slots[i].item.itemPrice.ToString();
                        continue;
                    }

                    int spaceRemaining = playerInventory.innInventory[index].InventorySpaceRemaining();
                    if (spaceRemaining < 5)
                    {
                        slots[i].itemName.text = slots[i].item.name + " (x" + spaceRemaining + ")";
                        slots[i].itemPriceText.text = (slots[i].item.itemPrice * spaceRemaining).ToString();
                    }

                    int amount = int.Parse(slots[i].itemAmount.text);
                    if (amount < 5)
                    {
                        slots[i].itemName.text = slots[i].item.name + " (x" + amount + ")";
                        slots[i].itemPriceText.text = (slots[i].item.itemPrice * amount).ToString();
                    }

                }
                else if (playerInventory.GetCoins() < slots[i].item.itemPrice)
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
