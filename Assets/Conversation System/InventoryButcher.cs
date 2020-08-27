using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButcher : NPCInventorySystem
{
    private ulong lastProductionTime = 0;
    private ulong nextProductionTime = 10480;
    private const int PRODUCTION_TIME = 720;

#pragma warning disable 0649
    [Header("Production Items")]
    [SerializeField] private Food_Item chicken;
    [SerializeField] private Food_Item beef;
    [SerializeField] private Food_Item lamb;
    [SerializeField] private Food_Item pork;
#pragma warning restore 0649

    // Start is called before the first frame update
    void Start()
    {
        shopInventory = ShopInventoryManager.Instance;
        playerInventory = PlayerInventory.Instance;
        gameTimeManager = GameTimeManager.Instance;

        //StartCoroutine(BuildUpResources());
        GenerateResources();
    }


    private void GenerateResources()
    {
        Debug.Log("Butcher is producing more items.");

        int index = FindItemInInventory(chicken);
        if (index >= 0 && inventorySlots[index].amount <= 4)
        {
            inventorySlots[index].amount += Random.Range(0, 4);
        }
        else if (index < 0)
        {
            inventorySlots.Add(new InventorySlotItem(chicken, Random.Range(0, 4)));
        }

        index = FindItemInInventory(beef);
        if (index >= 0 && inventorySlots[index].amount <= 4)
        {
            inventorySlots[index].amount += Random.Range(0, 4);
        }
        else if (index < 0)
        {
            inventorySlots.Add(new InventorySlotItem(beef, Random.Range(0, 4)));
        }


        index = FindItemInInventory(pork);
        if (index >= 0 && inventorySlots[index].amount <= 4)
        {
            inventorySlots[index].amount += Random.Range(0, 4);
        }
        else if (index < 0)
        {
            inventorySlots.Add(new InventorySlotItem(pork, Random.Range(0, 4)));
        }

        index = FindItemInInventory(lamb);
        if (index >= 0 && inventorySlots[index].amount <= 4)
        {
            inventorySlots[index].amount += Random.Range(0, 4);
        }
        else if (index < 0)
        {
            inventorySlots.Add(new InventorySlotItem(lamb, Random.Range(0, 4)));
        }

        lastProductionTime = gameTimeManager.GetWorldTime();
        nextProductionTime = gameTimeManager.GetNextWorldTime(lastProductionTime, PRODUCTION_TIME);

        StartCoroutine(gameTimeManager.WaitUntilWorldTime(nextProductionTime, GenerateResources));
        Debug.Log("Butcher will produce more items at: " + nextProductionTime);
    }

    //private IEnumerator BuildUpResources()
    //{
    //    while (true)
    //    {
    //        if (nextProductionTime <= gameTimeManager.GetWorldTime())
    //        {
    //            Debug.Log("Butcher is producing more items.");

    //            int index = FindItemInInventory(chicken);
    //            if (index >= 0 && inventorySlots[index].amount <= 4)
    //            {
    //                inventorySlots[index].amount += Random.Range(0, 4);
    //            }
    //            else if (index < 0)
    //            {
    //                inventorySlots.Add(new InventorySlotItem(chicken, Random.Range(0, 4)));
    //            }

    //            yield return null;

    //            index = FindItemInInventory(beef);
    //            if (index >= 0 && inventorySlots[index].amount <= 4)
    //            {
    //                inventorySlots[index].amount += Random.Range(0, 4);
    //            }
    //            else if (index < 0)
    //            {
    //                inventorySlots.Add(new InventorySlotItem(beef, Random.Range(0, 4)));
    //            }

    //            yield return null;

    //            index = FindItemInInventory(pork);
    //            if (index >= 0 && inventorySlots[index].amount <= 4)
    //            {
    //                inventorySlots[index].amount += Random.Range(0, 4);
    //            }
    //            else if (index < 0)
    //            {
    //                inventorySlots.Add(new InventorySlotItem(pork, Random.Range(0, 4)));
    //            }

    //            yield return null;

    //            index = FindItemInInventory(lamb);
    //            if (index >= 0 && inventorySlots[index].amount <= 4)
    //            {
    //                inventorySlots[index].amount += Random.Range(0, 4);
    //            }
    //            else if (index < 0)
    //            {
    //                inventorySlots.Add(new InventorySlotItem(lamb, Random.Range(0, 4)));
    //            }

    //            lastProductionTime = gameTimeManager.GetWorldTime();
    //            nextProductionTime = gameTimeManager.GetNextWorldTime(lastProductionTime, PRODUCTION_TIME);
    //            Debug.Log("Butcher will produce more items at: " + nextProductionTime);
    //        }

    //        yield return new WaitForSeconds(60f / gameTimeManager.GetTimeMultiplier());
    //    }
    //}

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
