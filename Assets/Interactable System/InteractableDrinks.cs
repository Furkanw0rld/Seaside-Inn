using UnityEngine;

public class InteractableDrinks : Interactable
{
    [SerializeField] private DrinkName drinkName = DrinkName.Ale;

    private InventorySlotItem drinkSlot;
    private PlayerInventory playerInventory;
    private DisplayMessageUI displayMessageUI;

    private new void Start()
    {
        if (PlayerInventory.Instance)
        {
            playerInventory = PlayerInventory.Instance;
            drinkSlot = playerInventory.innInventory[(int)drinkName];
        }

        displayMessageUI = DisplayMessageUI.Instance;
    }

    public override void Interact()
    {
        if(drinkSlot.amount > 0)
        {
            drinkSlot.RemoveAmount(1);
            playerInventory.onInventoryChangedCallback(InventoryType.InnInventory);
        }
        else
        {
            displayMessageUI.DisplayMessage("<color=#ffde00>" + drinkName.ToString() + "</color> is empty!");
        }
    }
}

public enum DrinkName //These IDs Follow the Drink's Position in the Inn Inventory. 
{
    Ale = 0,
    Rum = 1,
    Mead = 2,
    Wine = 3
}
