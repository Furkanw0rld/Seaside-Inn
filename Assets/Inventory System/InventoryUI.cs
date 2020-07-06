using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; //UI Inventory Panel
    public Transform inventorySlots;
    public Sprite genericIcon;
    public ItemInformationWindow informationWindow;

    private PlayerInventory playerInventory; //Cached inventory
    private InventorySlot[] slots;

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        playerInventory.onInventoryChangedCallback += UpdateUI;

        slots = inventorySlots.GetComponentsInChildren<InventorySlot>(); //Get all the inventory slots from scene

        foreach(InventorySlot slot in slots)
        {
            slot.amountText.enabled = false;
            slot.icon.enabled = false;
            slot.item = null;
            slot.informationWindow = informationWindow;
        }
    }

    private void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.activeSelf);
        }
    }

    private void OnDisable()
    {
        playerInventory.onInventoryChangedCallback -= UpdateUI;
    }

    private void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(i < playerInventory.inventory.Count) //Items in inventory
            {
                DisplayItemAtSlot(i);
            }
            else
            {
                EmptyItemSlot(i);
            }
        }
        Debug.Log("UI Updated.");
    }

    private void DisplayItemAtSlot(int index)
    {
        slots[index].amountText.enabled = true;
        slots[index].icon.enabled = true;
        slots[index].item = playerInventory.inventory[index].item;

        slots[index].amountText.text = playerInventory.inventory[index].amount.ToString();

        if (playerInventory.inventory[index].item.icon) // If the Item has an icon display it, otherwise show generic.
        {
            slots[index].icon.sprite = playerInventory.inventory[index].item.icon;
        }
        else
        {
            slots[index].icon.sprite = genericIcon;
        }
    }

    private void EmptyItemSlot(int index)
    {
        slots[index].amountText.enabled = false;
        slots[index].icon.enabled = false;
        slots[index].item = null;
    }

}
