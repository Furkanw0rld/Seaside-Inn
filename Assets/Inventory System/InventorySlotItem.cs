using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlotItem
{
    public Item item;
    public int amount;

    public InventorySlotItem(Item _item, int _amount)
    {
        item = _item;

        if (_amount > item.itemAmountLimit) //If the amount we are trying to add is more than the limit, then just add up to the limit.
        {
            amount = item.itemAmountLimit;
        }
        else //Otherwise add amount
        {
            amount = _amount;
        }

    }

    public bool AddAmount(int _amount)
    {
        amount += _amount;
        return true;

    }

    public bool RemoveAmount(int _amount)
    {
        if (amount - _amount > 0)
        {
            amount -= _amount;
            return true;
        }
        return false;
    }

    public int InventorySpaceRemaining()
    {
        return item.itemAmountLimit - amount;
    }
}