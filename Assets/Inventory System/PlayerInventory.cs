using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    private float coins = 1;
    private readonly float maximumCoinFillAmount = 250; //This the amount of coins when the UI Gold Bar is filled completely.
    private PlayerManager manager; //Cache of manager
    [Header("Player Inventory")]
    [SerializeField][Tooltip("How many slots does the player have")] private int inventorySpace = 15;
    public List<InventorySlotItem> inventory = new List<InventorySlotItem>();


    // PUBLIC METHODS:
    public float GetCoins()
    {
        return coins;
    }

    public float AddCoins(float amount)
    {
        coins += amount;
        manager.coinsText.text = coins.ToString();
        manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
        return coins;
    }

    public float RemoveCoins(float amount)
    {
        coins -= amount;
        manager.coinsText.text = coins.ToString();
        manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
        return coins;
    }

    private int IsThereAnotherItemWithSpace(int startingIndex, Item itemToCheck)
    {
        for(int i = startingIndex; i < inventory.Count; i++)
        {
            if(inventory[i].item == itemToCheck)
            {
                if(inventory[i].InventorySpaceRemaining() > 0)
                {
                    Debug.Log("Another item: Returning Index: " + i);
                    return i;
                }
            }
        }
        Debug.Log("No more. Returning -1");
        return -1;
    }

    public int AddItem(Item newItem, int amountToAdd)
    {
        bool hasItem = false;

        int amountLeftToAdd = amountToAdd;
        for(int i = 0; i < inventory.Count; i++)
        {
            if(amountLeftToAdd <= 0) //Added all items
            {
                break;
            }

            if(inventory[i].item == newItem) //Found a match
            {
                hasItem = true;
                if (inventory[i].InventorySpaceRemaining() >= amountToAdd) //We found the item, and there is room to add all of it
                {
                    inventory[i].AddAmount(amountToAdd);
                    Debug.Log("Added " + amountToAdd + " " + newItem.name);
                    amountLeftToAdd -= amountToAdd;
                }
                else //Not enough room to add all of it
                {
                    if(inventory[i].InventorySpaceRemaining() > 0) //Space left to add
                    {
                        int newAmountToAdd = inventory[i].InventorySpaceRemaining();
                        inventory[i].AddAmount(newAmountToAdd); //Add up to the inventory limit
                        amountLeftToAdd -= newAmountToAdd;
                    }

                    if (inventory.Count < inventorySpace) // We have more room in inventory to add to another slot
                    {
                        if (amountLeftToAdd > inventory[i].item.itemAmountLimit) // Trying to add even more than that's left
                        {
                            //Probably will never occur, skipping for now.
                        }
                        else
                        {
                            int newIndex = IsThereAnotherItemWithSpace(i, newItem);

                            if(newIndex >= 0)
                            {
                                if(inventory[i].InventorySpaceRemaining() <= amountLeftToAdd)
                                {
                                    inventory[newIndex].AddAmount(amountLeftToAdd);
                                    amountLeftToAdd -= amountLeftToAdd;
                                }
                            }
                            else
                            {
                                inventory.Add(new InventorySlotItem(newItem, amountLeftToAdd)); //Add it to a new slot, subtract from the amount already added.
                                amountLeftToAdd -= amountLeftToAdd;
                            }
                        }
                    }
                    else // No room left at all but might have an item slot that isn't full, check for one
                    {
                        int newIndex = IsThereAnotherItemWithSpace(i, newItem);

                        if (newIndex >= 0)
                        {
                            if (inventory[i].InventorySpaceRemaining() <= amountLeftToAdd)
                            {
                                inventory[newIndex].AddAmount(amountLeftToAdd);
                                amountLeftToAdd -= amountLeftToAdd;
                            }
                        }
                        return amountToAdd - amountLeftToAdd;
                    }
                }
            }
        }

        if (!hasItem) //We did not find item in inventory
        {
            if(inventory.Count >= inventorySpace) //There is no slots left in our inventory to add item to, return 0.
            {
                Debug.Log("Inventory is Full.");
                return 0;  
            }
            else //We have inventory slot available to add new item
            {
                if(newItem.itemAmountLimit < amountToAdd) //Make sure it is less than total slots
                {
                    amountToAdd = newItem.itemAmountLimit;
                }

                inventory.Add(new InventorySlotItem(newItem, amountToAdd));
                Debug.Log("New item! " + amountToAdd + "  " + newItem.name + " added!");
                return amountToAdd;
            }
        }

        return amountToAdd - amountLeftToAdd; 
    }

    public bool BuyItem(Item item, float costPerItem, int amountToAdd)
    {
        int amountAdded = 0;

        if (costPerItem * amountToAdd <= GetCoins())
        {
            amountAdded = AddItem(item, amountToAdd); //Try to add amount
            RemoveCoins(amountAdded * costPerItem); //Remove coins based on how many got added.
        }


        if(amountAdded > 0) //We purchased an item, update ui.
        {
            onInventoryChangedCallback?.Invoke();
        }

        return (amountAdded > 0); //Buying item is successful if we bought more than 0.
    }

    public bool RemoveItem(Item itemToRemove, int _amount)
    {
        for(int i = inventory.Count - 1; i > 0; i--)
        {
            if(inventory[i].item == itemToRemove)
            {
                onInventoryChangedCallback?.Invoke(); //We found the item, and will perform remove operation, update ui prior.

                if(inventory[i].amount <= _amount)
                {
                    inventory.RemoveAt(i);
                    return true;
                }
                else
                {
                    return(inventory[i].RemoveAmount(_amount));
                }
            }
        }

        return false; //Couldn't remove any items
    }

    public InventorySlotItem FindItemInInventory(Item item)
    {
        foreach(InventorySlotItem slot in inventory) // Try to find slot, if found return it.
        {
            if(slot.item == item) 
            {
                return slot;
            }
        }
        // Return null if not found
        return null; 
    }


    //PRIVATE METHODS:

    private void Awake()
    {
        if (Instance == null)
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
        manager = PlayerManager.Instance;
        manager.coinsText.text = coins.ToString();
        manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
    }
}
