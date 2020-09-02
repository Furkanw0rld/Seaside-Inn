using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDrinks : Interactable
{
    [SerializeField] private DrinkName drinkName = DrinkName.Ale;

    private InventorySlotItem drinkSlot;
    private PlayerInventory playerInventory;

    private new void Start()
    {
        if (PlayerInventory.Instance)
        {
            playerInventory = PlayerInventory.Instance;
            drinkSlot = playerInventory.innInventory[(int)drinkName];
        }
    }

    public override void Interact()
    {
        base.Interact();

        if(drinkSlot.amount > 0)
        {
            drinkSlot.RemoveAmount(1);
            playerInventory.onInventoryChangedCallback(InventoryType.InnInventory);
        }
        else
        {
            Debug.Log(drinkName.ToString() + " is empty!");
        }
    }

    private enum DrinkName //These IDs Follow the Drink's Position in the Inn Inventory. 
    {
        Ale = 0,
        Cider = 1,
        Mead = 2,
        Wine = 3
    }
}
