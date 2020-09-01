using UnityEngine;

[System.Serializable]
public class InventorySlotItem
{
    public Item item;
    [Min(1)] public int amount;

    public InventorySlotItem(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int _amount)
    {
        amount += _amount;
        if(amount > item.itemAmountLimit)
        {
            amount = item.itemAmountLimit;
        }
    }

    public void RemoveAmount(int _amount)
    {
        amount -= _amount;
    }

    public int InventorySpaceRemaining()
    {
        return item.itemAmountLimit - amount;
    }
}