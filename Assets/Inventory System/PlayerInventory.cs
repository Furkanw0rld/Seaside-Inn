using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	public static PlayerInventory Instance;
	public delegate void OnInventoryChanged(InventoryType inventoryType);
	public OnInventoryChanged onInventoryChangedCallback;

	private float coins = 100;
	private readonly float maximumCoinFillAmount = 250; //This the amount of coins when the UI Gold Bar is filled completely.
	private PlayerManager manager; //Cache of manager
	[Header("Player Inventory")]
	[Tooltip("How many slots does the PLAYER have")] private const int INVENTORY_SPACE = 12;
	[Tooltip("How many slots does the INN have")] private const int INN_INVENTORY_SPACE = 20;
	[Tooltip("How many slots does the TRADE have")] private const int TRADE_INVENTORY_SPACE = 20;

	[Tooltip("Player Inventory.")] public List<InventorySlotItem> inventory = new List<InventorySlotItem>();
	[Tooltip("Inn Inventory.")] public List<InventorySlotItem> innInventory = new List<InventorySlotItem>();
	[Tooltip("Trade Inventory.")] public List<InventorySlotItem> tradeInventory = new List<InventorySlotItem>();

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

	private int FindAnotherItemInInnWithSpace(int startingIndex, Item itemToCheck)
	{
		switch (itemToCheck)
		{
			case Food_Item foodItem:
				for(int i = startingIndex; i < innInventory.Count; i++)
				{
					if(innInventory[i].item.name == foodItem.name) // Matching food item
					{
						if(innInventory[i].InventorySpaceRemaining() > 0) // There is space available
						{
							Food_Item inventoryFoodItem = (Food_Item)innInventory[i].item;
							if(inventoryFoodItem.freshness == foodItem.freshness) 
							{
								// This is a perfect match, add here.
								return i;
							}
						}
					}
				}
				break;

			case Drink_Item drinkItem:
				if(startingIndex > 3) // Drink Items are a SPECIAL CASE and are reserved the first 4 Slots in inventory. 
				{
					return -1;
				}
				switch (drinkItem.name) // Drink items are known as these, anything not matching these names, are not drink items.
				{
					case "Ale":
						if(innInventory[0].InventorySpaceRemaining() > 0)
						{
							return 0;
						}
						break;
					case "Cider":
						if(innInventory[1].InventorySpaceRemaining() > 0)
						{
							return 1;
						}
						break;
					case "Mead":
						if(innInventory[2].InventorySpaceRemaining() > 0)
						{
							return 2;
						}
						break;
					case "Wine":
						if(innInventory[3].InventorySpaceRemaining() > 0)
						{
							return 3;
						}
						break;
					default:
						return -1;
				}
				break;
			default: // Inn Inventory Only Can Add Food or Drink Items
				break;
		}

		//Did not manage to find a matching item.
		return -1;
	}

	private int FindAnotherItemWithSpace(int startingIndex, Item itemToCheck)
	{
		switch (itemToCheck)
		{
			case Food_Item foodItem:
				for(int i = startingIndex; i < inventory.Count; i++)
				{
					if(inventory[i].item.name == foodItem.name) //We found a matching food item.
					{
						if(inventory[i].InventorySpaceRemaining() > 0) // There is space available for this food item, but need to check if this items quality matches
						{
							Food_Item inventoryFoodItem = (Food_Item)inventory[i].item;
							if(inventoryFoodItem.freshness == foodItem.freshness) 
							{
								return i;
								//We have a perfect match with freshness, can add here.
							}
						}
					}
				}
				break;

			case Drink_Item _:
				return -1; // Can't add Drink Items to Player Inventory.

			default: //Not a special case. 
				for (int i = startingIndex; i < inventory.Count; i++)
				{
					if (inventory[i].item.name == itemToCheck.name) //We are using the name parameter to check, since we are instantiating new items now.
					{
						if (inventory[i].InventorySpaceRemaining() > 0)
						{
							return i;
						}
					}
				}
				break;
		}

		//We did not find a matching case for our item, return -1.
		return -1;
	}

	private int FindAnotherItemInTradeWithSpace(int startingIndex, Item itemToCheck)
	{
		for(int i = startingIndex; i < tradeInventory.Count; i++)
        {
			if(tradeInventory[i].item.name == itemToCheck.name)
            {
				if(tradeInventory[i].InventorySpaceRemaining() > 0)
                {
					return i; //Found item return index
                }
            }
        }

		return -1; //Didn't find, return -1
	}

	public bool AddItem(Item newItem) // ADDS ITEM TO PLAYER INVENTORY 
	{
		bool addedItem = false;

		for(int i = 0; i < inventory.Count; i++)
		{

			if(inventory[i].item.name == newItem.name) //Found a matching item
			{
				
				if(inventory[i].InventorySpaceRemaining() > 0) // There is room to add item here.
				{
					if(newItem.itemType == ItemType.Food) // Special Case, we are trying to add a food item.
					{
						Food_Item newFoodItem = (Food_Item)newItem;
						Food_Item currentFoodItem = (Food_Item)inventory[i].item;
						if(newFoodItem.freshness == currentFoodItem.freshness) // The Foods freshness are the same.
						{
							inventory[i].AddAmount(1);
							addedItem = true;
							break;
						}
						else // The freshness is not the same, try to find another item with slot to add to
						{
							continue;
						}
					}
					else // Not a special case and we know there is space available to add.
					{
						inventory[i].AddAmount(1);
						addedItem = true;
						break; // Add to slot and break.
					}

				}
			}
		}

		if (!addedItem)
		{
			if(inventory.Count >= INVENTORY_SPACE) // No room in inventory.
			{
				return addedItem;
			}
			else // We can add to another slot.
			{
				inventory.Add(new InventorySlotItem(Instantiate(newItem), 1));
				addedItem = true;
				return addedItem;
			}
		}

		return addedItem;
	}

	public bool AddItem(Item item, InventoryType inventoryTo) //Overload to add to a specific inventory 
	{
		bool addedItem = false;

		switch (inventoryTo)
		{
			case InventoryType.PlayerInventory: //If we are trying to add to player inventory perform regular add operation.
				return AddItem(item);

			case InventoryType.InnInventory:
				if(item.itemType != ItemType.Food && item.itemType != ItemType.Drink)
				{
					break; 
					// We can't add anything but food or drink to inn.
				}

				for (int i = 0; i < innInventory.Count; i++)
				{
					if (innInventory[i].item.name == item.name) // Matching Item
					{
						if (innInventory[i].InventorySpaceRemaining() > 0) //Space available to add
						{
							if (item.itemType == ItemType.Food) // Food Item
							{
								Food_Item foodItem = (Food_Item)item;
								Food_Item inventoryItem = (Food_Item)innInventory[i].item;

								if (foodItem.freshness == inventoryItem.freshness) // Matching freshness, add item.
								{
									innInventory[i].AddAmount(1);
									addedItem = true;
									break;
								}
								else // Freshness does not match, try to find another.
								{
									continue; 
								}
							}
							else // Everything else must be a drink item since we do a check when we enter the case.
							{
								innInventory[i].AddAmount(10); // Just adding 10 for now. 
								addedItem = true;
								break;
							}
						}
					}
				}

                if (!addedItem) // Didn't manage to add the item.
                {
					if(innInventory.Count >= INN_INVENTORY_SPACE) // No room in inventory.
                    {
						return addedItem;
                    }
                    else
                    {
						innInventory.Add(new InventorySlotItem(Instantiate(item), 1));
						addedItem = true;
						return addedItem;
                    }
                }

				break;
			case InventoryType.TradeInventory:
				//TODO: Implement Trade Inventory Add Mechanism 
				//This functionality probably is not needed, skipping for now.
				Debug.Log("Trade Inventory not yet implemented.");
				break;
		}

		return false;
	}

    public void MoveItem(Item item, int amount, InventoryType inventoryTo)
    {
        if (!CanMoveItem(inventoryTo)) //If there is no space to move, just break out without continuing.
        {
			//TODO: Should probably check if it's possible to add items to onto another before returning, thus we could partially add items to another slot. Not a priority for now.
			return;
		}

        switch (inventoryTo)
        {
            case InventoryType.PlayerInventory:

                if (item.itemType == ItemType.Drink) // Can't Move Drinks to player inventory.
                {
                    return;
                }

                int playerIndex = FindAnotherItemWithSpace(0, item); // Try to find another item in inventory first.
                int amountAddedToPlayer = 0;

                if (playerIndex >= 0) // Found a matching slot.
                {
                    if (inventory[playerIndex].InventorySpaceRemaining() >= amount) //Enough space to add items onto player slot
                    {
                        inventory[playerIndex].AddAmount(amount);
                        onInventoryChangedCallback?.Invoke(inventoryTo);
                        if (item.itemType == ItemType.Food) //Means we took away from inn
                        {
                            RemoveItem(item, InventoryType.InnInventory, amount); //Remove from inn
                        }
                        else
                        {
                            RemoveItem(item, InventoryType.TradeInventory, amount); //Remove from trade
                        }
                        return; // Managed to add all items to another slot. No need to continue

                    }
                    else //Not enough room to add all items, but room to add some
                    {
                        amountAddedToPlayer = inventory[playerIndex].InventorySpaceRemaining();
                        inventory[playerIndex].AddAmount(amountAddedToPlayer);
                    }
                    inventory.Add(new InventorySlotItem(Instantiate(item), amount - amountAddedToPlayer));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    if (item.itemType == ItemType.Food) //Means we took away from inn
                    {
                        RemoveItem(item, InventoryType.InnInventory, amount); //Remove from inn
                    }
                    else
                    {
                        RemoveItem(item, InventoryType.TradeInventory, amount); //Remove from trade
                    }
                    return;
                }
                else // Did not find a matching slot.
                {
                    inventory.Add(new InventorySlotItem(Instantiate(item), amount));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    if (item.itemType == ItemType.Food) //Means we took away from inn
                    {
                        RemoveItem(item, InventoryType.InnInventory, amount); //Remove from inn
                    }
                    else
                    {
                        RemoveItem(item, InventoryType.TradeInventory, amount); //Remove from trade
                    }
                    return;
                }

            case InventoryType.InnInventory:
                if (item.itemType != ItemType.Food)
                {
                    return; //Can only add Food Items (Drinks are always added to inn inventory, and can't be moved)
                }

                int innIndex = FindAnotherItemInInnWithSpace(4, item); // First try to find another item.
                int amountAddedToInn = 0;

                if (innIndex >= 4) //Found a possible slot.
                {
                    if (innInventory[innIndex].InventorySpaceRemaining() >= amount) // There is enough space to add all items.
                    {
                        innInventory[innIndex].AddAmount(amount);
                        onInventoryChangedCallback?.Invoke(inventoryTo);
                        RemoveItem(item, InventoryType.PlayerInventory, amount); // Remove item from main inventory
                        return;
                    }
                    else // Not enough room to add items.
                    {
                        amountAddedToInn = innInventory[innIndex].InventorySpaceRemaining(); // Get amount of space left in slot
                        innInventory[innIndex].AddAmount(amountAddedToInn); // Add up to it.
                    }
                    // Create a new slot and add the rest.
                    innInventory.Add(new InventorySlotItem(Instantiate(item), amount - amountAddedToInn));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    RemoveItem(item, InventoryType.PlayerInventory, amount); // Remove item from main inventory
                    return;
                }
                else // Did not find a possible slot.
                {
                    innInventory.Add(new InventorySlotItem(Instantiate(item), amount));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    RemoveItem(item, InventoryType.PlayerInventory, amount); // Remove item from main inventory
                    return;
                }

            case InventoryType.TradeInventory:

                int tradeIndex = FindAnotherItemInTradeWithSpace(0, item);
                int amountAddedToTrade = 0;

                if (tradeIndex >= 0) // Found possible slot
                {
                    if (tradeInventory[tradeIndex].InventorySpaceRemaining() >= amount) //Enough room to add all of it
                    {
                        tradeInventory[tradeIndex].AddAmount(amount);
                        onInventoryChangedCallback?.Invoke(inventoryTo);
                        RemoveItem(item, InventoryType.TradeInventory, amount);
                        return; //We are done and can exit
                    }
                    else //Not enough room to add all but can add some
                    {
                        amountAddedToTrade = tradeInventory[tradeIndex].InventorySpaceRemaining();
                        tradeInventory[tradeIndex].AddAmount(amountAddedToTrade);
                    }

                    tradeInventory.Add(new InventorySlotItem(Instantiate(item), amount - amountAddedToTrade));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    RemoveItem(item, InventoryType.PlayerInventory, amount);
                    return;
                }
                else //Didn't manage to find a possible slot
                {
					//Add to a new slot with the amount.
                    tradeInventory.Add(new InventorySlotItem(Instantiate(item), amount));
                    onInventoryChangedCallback?.Invoke(inventoryTo);
                    RemoveItem(item, InventoryType.PlayerInventory, amount);
                    return;
                }
        }
    }

    public bool CanMoveItem(InventoryType inventoryTo)
    {
		switch (inventoryTo)
		{
			case InventoryType.PlayerInventory:
				return !(inventory.Count >= INVENTORY_SPACE);

			case InventoryType.InnInventory:
				return !(innInventory.Count >= INN_INVENTORY_SPACE);

			case InventoryType.TradeInventory:
				return !(tradeInventory.Count >= TRADE_INVENTORY_SPACE);
			default:
				return false;
		}
	}

    private void RemoveItem(Item item, InventoryType inventoryFrom, int currentItemAmount)
    {
        switch (inventoryFrom)
        {
            case InventoryType.PlayerInventory:
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].item.name == item.name) // Found a match
                    {
                        if (inventory[i].item.itemType == ItemType.Food) // Food must also match freshness
                        {
                            if (inventory[i].amount == currentItemAmount) // New safety check to make sure we matching the correct item amount.
                            {
                                Food_Item foodItem = (Food_Item)item;
                                Food_Item inventoryItem = (Food_Item)inventory[i].item;
                                if (foodItem.freshness == inventoryItem.freshness) // Matching item, delete.
                                {
                                    inventory.RemoveAt(i);
                                    onInventoryChangedCallback?.Invoke(inventoryFrom);
                                    return;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else // Every other item
                        {
                            if (inventory[i].amount == currentItemAmount) // Another safety check to make sure we are removing the matching item with the same amount.
                            {
                                inventory.RemoveAt(i);
                                onInventoryChangedCallback?.Invoke(inventoryFrom);
                                return;
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                }
                break;

            case InventoryType.InnInventory:
                if (item.itemType == ItemType.Drink)
                {
                    return; //Can't delete drinks out of inventory.
                }

                for (int i = 4; i < innInventory.Count; i++)
                {
                    if (innInventory[i].item.name == item.name) //Found a matching item
                    {
                        if (innInventory[i].amount == currentItemAmount) //Safety check to equate the amount
                        {
                            Food_Item foodItem = (Food_Item)item;
                            Food_Item inventoryItem = (Food_Item)innInventory[i].item;

                            if (foodItem.freshness == inventoryItem.freshness) //Matching freshness
                            {
                                innInventory.RemoveAt(i);
                                onInventoryChangedCallback?.Invoke(inventoryFrom);
                                return;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
                break;

            case InventoryType.TradeInventory:
                for (int i = 0; i < tradeInventory.Count; i++)
                {
                    if (tradeInventory[i].item.name == item.name) //Name match
                    {
                        if (tradeInventory[i].amount == currentItemAmount) // Amount match
                        {
                            tradeInventory.RemoveAt(i);
                            onInventoryChangedCallback?.Invoke(inventoryFrom);
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                break;
        }
    }

    public bool BuyItem(Item item, float cost)
	{
		bool boughtItem = false;

		if(cost <= GetCoins()) // We have coins to buy item
		{
			boughtItem = AddItem(item);
			if (boughtItem) //We managed to buy it, take away coins and update UI.
			{
				RemoveCoins(cost);
				onInventoryChangedCallback?.Invoke(InventoryType.PlayerInventory); 
			}
		}

		return boughtItem;
	}

	public void RemoveItemAtIndex(int index, InventoryType inventoryFrom)
	{
        switch (inventoryFrom)
        {
			case InventoryType.PlayerInventory:
				Destroy(inventory[index].item);
				inventory.RemoveAt(index);
				onInventoryChangedCallback?.Invoke(inventoryFrom);
				break;

			case InventoryType.InnInventory:
				Destroy(innInventory[index].item);
				innInventory.RemoveAt(index);
				onInventoryChangedCallback?.Invoke(inventoryFrom);
				break;

			case InventoryType.TradeInventory:
				Destroy(tradeInventory[index].item);
				tradeInventory.RemoveAt(index);
				onInventoryChangedCallback?.Invoke(inventoryFrom);
				break;
        }
	}

    public void RemoveSpoiltFoodFromInventory()
    {
		for (int i = 4; i < innInventory.Count; i++)
		{
			if (innInventory[i].item.itemType == ItemType.Food)
			{
				Food_Item food = (Food_Item)innInventory[i].item;
				if (food.freshness == FoodFreshness.Spoilt)
				{
					RemoveItemAtIndex(i, InventoryType.InnInventory);
					continue;
                }
                else
                {
					continue;
                }
			}
		}

		for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item.itemType == ItemType.Food)
            {
                Food_Item food = (Food_Item)inventory[i].item;
                if (food.freshness == FoodFreshness.Spoilt)
                {
                    RemoveItemAtIndex(i, InventoryType.PlayerInventory);
					continue;
                }
                else
                {
					continue;
                }
            }
        }
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
public enum InventoryType
{
    PlayerInventory,
    InnInventory,
    TradeInventory
}