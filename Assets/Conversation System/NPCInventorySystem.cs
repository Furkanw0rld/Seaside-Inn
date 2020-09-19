using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class NPCInventorySystem : MonoBehaviour
{

    public List<InventorySlotItem> inventorySlots = new List<InventorySlotItem>();
    protected ShopInventoryManager shopInventory;
    protected PlayerInventory playerInventory;
    protected GameTimeManager gameTimeManager;

    public abstract void OpenShop();
    public abstract void CloseShop();
    public abstract void BuyItem(InventorySlotItem slot, Button purchaseButton, NPC_ItemSlotInformationData slotInformation);
}
