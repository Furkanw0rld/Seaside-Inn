using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	public static PlayerInventory Instance;
	public delegate void OnInventoryChanged(InventoryType inventoryType);
	public OnInventoryChanged onInventoryChangedCallback;

	private float coins = 1000;
	private readonly float maximumCoinFillAmount = 250; //This the amount of coins when the UI Gold Bar is filled completely.
	private PlayerManager manager; //Cache of manager
	[Header("Player Inventory")]
	[Tooltip("How many slots does the PLAYER have")] private const int INVENTORY_SPACE = 12;
	[Tooltip("How many slots does the INN have")] private const int INN_INVENTORY_SPACE = 20;
	[Tooltip("How many slots does the TRADE have")] private const int TRADE_INVENTORY_SPACE = 20;

	[Header("Inventories")]
	[Tooltip("Player Inventory.")] public List<InventorySlotItem> inventory = new List<InventorySlotItem>();
	[Tooltip("Inn Inventory.")] public List<InventorySlotItem> innInventory = new List<InventorySlotItem>();
	[Tooltip("Trade Inventory.")] public List<InventorySlotItem> tradeInventory = new List<InventorySlotItem>();

	[Header("Inn Inventory Starting Items")]
	public Item ale;
	public Item cider;
	public Item mead;
	public Item wine;

	[Header("Shelf System")]
#pragma warning disable 0649
	[SerializeField] private Transform innInventoryShelfSystem;
#pragma warning restore 0649
	private Transform[] innInventoryShelves;

	[Header("Recipe System")]
	public Dictionary<string, int> inventoryFoodTracker = new Dictionary<string, int>(); //Dynamically tracks all food items in all (inn/player) inventories

	// PUBLIC METHODS:
	public float GetCoins()
	{
		return coins;
	}

	public float AddCoins(float amount)
	{
		coins += amount;
		manager.coinsText.text = string.Format("{0:0.##}", coins);
		manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;
		return coins;
	}

	public float RemoveCoins(float amount)
	{
		coins -= amount;
		manager.coinsText.text = string.Format("{0:0.##}", coins);
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
				Debug.Log("Directly adding items to Trade Inventory not yet implemented.");
				break;
		}

		return false;
	}

	private void RemoveAmountFromItem(Item item, int amountToRemove, InventoryType inventoryFrom)
    {
		int amountRemoved = 0;

		switch (inventoryFrom)
        {
			case InventoryType.PlayerInventory:
				switch (item)
				{
					case Food_Item foodItem:
						
						for(int i = 0; i < inventory.Count; i++)
                        {
							if(amountRemoved >= amountToRemove)
                            {
								onInventoryChangedCallback?.Invoke(inventoryFrom);
								return;
                            }

							if(inventory[i].item.name == foodItem.name)
                            {
								Food_Item inventoryItem = (Food_Item)inventory[i].item;

								if(inventoryItem.freshness == foodItem.freshness)
                                {
									if(inventory[i].amount >= (amountToRemove - amountRemoved))
                                    {
										int toRemove = amountToRemove - amountRemoved;
										inventory[i].RemoveAmount(toRemove);
										amountRemoved += toRemove;
                                    }
                                    else
                                    {
										int toRemove = inventory[i].amount;
										RemoveItemAtIndex(i, InventoryType.PlayerInventory);
										amountRemoved += toRemove;
                                    }
                                }
                            }
                        }
						onInventoryChangedCallback?.Invoke(inventoryFrom);
						break;

					default:
						for (int i = 0; i < inventory.Count; i++)
                        {
							if(amountRemoved >= amountToRemove)
                            {
								onInventoryChangedCallback?.Invoke(inventoryFrom);
								return;
                            }

							if(inventory[i].item.name == item.name)
                            {
								if(inventory[i].amount >= (amountToRemove - amountRemoved))
                                {
									int toRemove = amountToRemove - amountRemoved;
									inventory[i].RemoveAmount(toRemove);
									amountRemoved += toRemove;
                                }
                                else
                                {
									int toRemove = inventory[i].amount;
									RemoveItemAtIndex(i, InventoryType.PlayerInventory);
									amountRemoved += toRemove;
                                }
                            }
                        }
						onInventoryChangedCallback?.Invoke(inventoryFrom);
						break;
                }
				break;

			case InventoryType.InnInventory:

				Food_Item fItem = (Food_Item)item;
				for(int i = 4; i < innInventory.Count; i++)
                {
					if(amountRemoved >= amountToRemove)
                    {
						onInventoryChangedCallback?.Invoke(inventoryFrom);
						return;
                    }

					if(innInventory[i].item.name == fItem.name)
                    {
						Food_Item inventoryItem = (Food_Item)innInventory[i].item;

						if(inventoryItem.freshness == fItem.freshness)
                        {
							if(innInventory[i].amount >= (amountToRemove - amountRemoved))
                            {
								int toRemove = amountToRemove - amountRemoved;
								innInventory[i].RemoveAmount(toRemove);
								amountRemoved += toRemove;
                            }
                            else
                            {
								int toRemove = innInventory[i].amount;
								RemoveItemAtIndex(i, InventoryType.InnInventory);
								amountRemoved += toRemove;
                            }
                        }
                    }
                }
				onInventoryChangedCallback?.Invoke(inventoryFrom);
				break;

			case InventoryType.TradeInventory:
				for(int i = 0; i < tradeInventory.Count; i++)
                {
					if(amountRemoved >= amountToRemove)
                    {
						onInventoryChangedCallback?.Invoke(inventoryFrom);
						return;
                    }

					if(tradeInventory[i].item.name == item.name)
                    {
						if (tradeInventory[i].amount >= (amountToRemove - amountRemoved))
						{
							int toRemove = amountToRemove - amountRemoved;
							tradeInventory[i].RemoveAmount(toRemove);
							amountRemoved += toRemove;
						}
						else
						{
							int toRemove = tradeInventory[i].amount;
							RemoveItemAtIndex(i, InventoryType.TradeInventory);
							amountRemoved += toRemove;
						}
					}
                }
				onInventoryChangedCallback?.Invoke(inventoryFrom);
				break;
        }
    }

    public void MoveItem(Item item, int amount, InventoryType inventoryTo)
    {
        if (!CanMoveItem(inventoryTo)) //If there is no space to move, nove item partially to another slot.
        {
            int foundIndex;

            switch (inventoryTo)
            {
				case InventoryType.PlayerInventory: //Player Inventory is full and we are trying to add an item here.
					foundIndex = FindAnotherItemWithSpace(0, item);

					if(foundIndex >= 0) // We found a slot we can add this item to. 
                    {
						int spaceRemaining = inventory[foundIndex].InventorySpaceRemaining();

						if(spaceRemaining >= amount) //We have enough room to add all of the item
                        {
							inventory[foundIndex].AddAmount(amount);

							if(item.itemType == ItemType.Food)
                            {
								RemoveItem(item, InventoryType.InnInventory, amount);
                            }
                            else
                            {
								RemoveItem(item, InventoryType.TradeInventory, amount);
                            }
                        }
                        else // Not enough room to add all of the item, add partially
                        {
							inventory[foundIndex].AddAmount(spaceRemaining);
							if(item.itemType == ItemType.Food)
                            {
								RemoveAmountFromItem(item, spaceRemaining, InventoryType.InnInventory);
                            }
                            else
                            {
								RemoveAmountFromItem(item, spaceRemaining, InventoryType.TradeInventory);
                            }
                        }
						onInventoryChangedCallback?.Invoke(inventoryTo);
                    }
                    else
                    {
						onInventoryChangedCallback?.Invoke(inventoryTo);
						return;
                    }
					break;

				case InventoryType.InnInventory:
					foundIndex = FindAnotherItemInInnWithSpace(4, item);
					if(foundIndex >= 0)
                    {
						int spaceRemaining = innInventory[foundIndex].InventorySpaceRemaining();
						if(spaceRemaining >= amount)
                        {
							innInventory[foundIndex].AddAmount(amount);
							RemoveItem(item, InventoryType.PlayerInventory, amount);
                        }
                        else
                        {
							innInventory[foundIndex].AddAmount(spaceRemaining);
							RemoveAmountFromItem(item, spaceRemaining, InventoryType.PlayerInventory);
                        }

						onInventoryChangedCallback?.Invoke(inventoryTo);
					}
                    else
                    {
						onInventoryChangedCallback?.Invoke(inventoryTo);
						return;
                    }
					break;

				case InventoryType.TradeInventory:
					foundIndex = FindAnotherItemInTradeWithSpace(0, item);
					if(foundIndex >= 0)
                    {
						int spaceRemaining = tradeInventory[foundIndex].InventorySpaceRemaining();
						if(spaceRemaining >= amount)
                        {
							tradeInventory[foundIndex].AddAmount(amount);
							RemoveItem(item, InventoryType.PlayerInventory, amount);
                        }
                        else
                        {
							tradeInventory[foundIndex].AddAmount(spaceRemaining);
							RemoveAmountFromItem(item, spaceRemaining, InventoryType.PlayerInventory);
                        }

						onInventoryChangedCallback?.Invoke(inventoryTo);
					}
                    else
                    {
						onInventoryChangedCallback?.Invoke(inventoryTo);
						return;
                    }
					break;

            }

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

    private void RemoveItem(Item item, InventoryType inventoryFrom, int currentItemAmount) //Remove item completely based on the amount there currently is.
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

				if(item is Food_Item) // Add this item to the food tracker if it's a food item.
                {
                    if (!inventoryFoodTracker.ContainsKey(item.name))
                    {
						inventoryFoodTracker.Add(item.name, 0);
                    }
					inventoryFoodTracker[item.name] += 1;
                }
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
					inventoryFoodTracker.Remove(innInventory[i].item.name);
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
					inventoryFoodTracker.Remove(inventory[i].item.name);
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

	private struct FoodItemsInInventoryHelperTracker
    {
		public int index;
		public InventoryType inventoryAt;
		public FoodFreshness freshness;
		public int amount;

		public FoodItemsInInventoryHelperTracker(int i, int amount, InventoryType type, FoodFreshness freshness)
        {
			index = i;
			this.amount = amount;
			inventoryAt = type;
			this.freshness = freshness;
        }
    }

	private List<FoodItemsInInventoryHelperTracker> GetFoodInInventoriesSorted(Food_Item item)
    {
		List<FoodItemsInInventoryHelperTracker> foodsFound = new List<FoodItemsInInventoryHelperTracker>();

		for (int i = 4; i < innInventory.Count; i++)
		{
			if (innInventory[i].item.name == item.name)
			{
				Food_Item fItem = (Food_Item)innInventory[i].item;
				foodsFound.Add(new FoodItemsInInventoryHelperTracker(i, innInventory[i].amount, InventoryType.InnInventory, fItem.freshness));
			}
		}
		for (int i = 0; i < inventory.Count; i++)
		{
			if (inventory[i].item.name == item.name)
			{
				Food_Item fItem = (Food_Item)inventory[i].item;
				foodsFound.Add(new FoodItemsInInventoryHelperTracker(i, inventory[i].amount, InventoryType.PlayerInventory, fItem.freshness));
			}
		}

		return foodsFound.OrderByDescending(x => x.freshness).ThenBy(x => x.amount).ToList();
	}

	public void FindAndUseFoodItem(Food_Item item, int amountToUse)
    {
		int amountUsed = 0;
		List<FoodItemsInInventoryHelperTracker> sortedFoods = GetFoodInInventoriesSorted(item);

		if(sortedFoods.Count > 1)
        {
			int i = 0;
			while(amountUsed < amountToUse)
            {
                switch (sortedFoods[i].inventoryAt)
                {
					case InventoryType.PlayerInventory:
						if(inventory[sortedFoods[i].index].amount >= (amountToUse - amountUsed))
                        {
							int used = amountToUse - amountUsed;
							inventory[sortedFoods[i].index].RemoveAmount(used);
							amountUsed += used;

							if(inventory[sortedFoods[i].index].amount <= 0)
                            {
								RemoveItemAtIndex(sortedFoods[i].index, InventoryType.PlayerInventory);
								sortedFoods = GetFoodInInventoriesSorted(item);
								i = 0;
                            }
                            else
                            {
								i++;
                            }
                        }
                        else
                        {
							int used = inventory[sortedFoods[i].index].amount;
							RemoveItemAtIndex(sortedFoods[i].index, InventoryType.PlayerInventory);
							sortedFoods = GetFoodInInventoriesSorted(item);
							amountUsed += used;
							i = 0;
                        }
						break;

					case InventoryType.InnInventory:
						if(innInventory[sortedFoods[i].index].amount >= (amountToUse - amountUsed))
                        {
							int used = amountToUse - amountUsed;
							innInventory[sortedFoods[i].index].RemoveAmount(used);
							amountUsed += used;

							if(innInventory[sortedFoods[i].index].amount <= 0)
                            {
								RemoveItemAtIndex(sortedFoods[i].index, InventoryType.InnInventory);
								sortedFoods = GetFoodInInventoriesSorted(item);
								i = 0;
                            }
                            else
                            {
								i++;
                            }
                        }
                        else
                        {
							int used = innInventory[sortedFoods[i].index].amount;
							RemoveItemAtIndex(sortedFoods[i].index, InventoryType.InnInventory);
							amountUsed += used;
							sortedFoods = GetFoodInInventoriesSorted(item);
							i = 0;
                        }
						break;
                }
            }
        }
        else
        {
            switch (sortedFoods[0].inventoryAt)
            {
				case InventoryType.PlayerInventory:
					inventory[sortedFoods[0].index].RemoveAmount(amountToUse);

					if(inventory[sortedFoods[0].index].amount <= 0)
                    {
						RemoveItemAtIndex(sortedFoods[0].index, InventoryType.PlayerInventory);
                    }
					break;

				case InventoryType.InnInventory:
					innInventory[sortedFoods[0].index].RemoveAmount(amountToUse);

					if(innInventory[sortedFoods[0].index].amount <= 0)
                    {
						RemoveItemAtIndex(sortedFoods[0].index, InventoryType.InnInventory);
                    }
					break;
            }
        }

		onInventoryChangedCallback?.Invoke(InventoryType.InnInventory);
		onInventoryChangedCallback?.Invoke(InventoryType.PlayerInventory);

		inventoryFoodTracker[item.name] -= amountToUse;
		if (inventoryFoodTracker[item.name] <= 0)
		{
			inventoryFoodTracker.Remove(item.name);
		}

	}

	//TODO: Improve the Inn Inventory Shelving System.
	// This base system lacks rotation,
	// and can probably made more efficient by caching subsystems and the GO's. 
	// Also, can improve this by incrementing the amount shown when there is multiple copies of item.
	// It might make sense to move this to a more performant class of it's own. 
	// Thereby, we can control each object, it's amount prefab, and it won't be derivately linked to the UI.
	// Currently, this system, basically copies the Inventory UI System, to replicate it to the screen.
	// This is unneccessary and uses performance when it does not need to
	// We delete each object each time and instantiate it again, we could save a lot of cycles, if this system
	// was more of a Dictionary, that kept each item at it's position until it was emptied. Thus, we don't have to call GC 
	// everytime there is a UI Callback. The performance impact as is can be up to 1 ms, which can probably be reduced down to few cycles, if we used a Data Oriented Approach. 

	private GameObject[] shelfItems = new GameObject[16];
	private void DisplayItemsAtShelf(InventoryType inventoryType)
    {
		if(inventoryType == InventoryType.InnInventory) // A Change occurred in inn inventory
        {
			for(int i = 0; i < innInventoryShelves.Length; i++)
            {
				if(i + 4 < innInventory.Count) // This is an item we can display
                {
                    if (innInventory[i+4].item.prefab)
                    {
                        if (shelfItems[i] != null)
                        {
							Destroy(shelfItems[i].gameObject);
							shelfItems[i] = null;
                        }
						shelfItems[i] = Instantiate(innInventory[i+4].item.prefab, innInventoryShelves[i].position, Quaternion.identity);
                    }
                    else
                    {
						Debug.Log("Item: " + innInventory[i+4].item.name + "has no gameObject skipping!");
                    }
                }
                else //Empty this shelf slot
                {
                    if (shelfItems[i] != null)
                    {
						Destroy(shelfItems[i]);
						shelfItems[i] = null;
                    }
                }
            }
        }
        else
        {
			return;
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

		// Add the starting items to inventory.
		innInventory.Add(new InventorySlotItem(Instantiate(ale), 1));
		innInventory.Add(new InventorySlotItem(Instantiate(cider), 10));
		innInventory.Add(new InventorySlotItem(Instantiate(mead), 20));
		innInventory.Add(new InventorySlotItem(Instantiate(wine), 38));
	}

	private void Start()
	{
		manager = PlayerManager.Instance;
		manager.coinsText.text = coins.ToString();
		manager.coinsFill.fillAmount = coins / maximumCoinFillAmount;


		//Shelf System, add all shelf areas to the array.
		innInventoryShelves = new Transform[innInventoryShelfSystem.childCount];
		for(int i = 0; i < innInventoryShelves.Length; i++)
        {
			innInventoryShelves[i] = innInventoryShelfSystem.GetChild(i);
        }
		//Add Shelf Displayer to the callback
		onInventoryChangedCallback += DisplayItemsAtShelf;

	}
}
public enum InventoryType
{
    PlayerInventory,
    InnInventory,
    TradeInventory
}