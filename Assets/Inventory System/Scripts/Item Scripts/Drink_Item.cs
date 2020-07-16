using UnityEngine;

//[CreateAssetMenu(fileName = "New Drink", menuName = "Inventory/New Drink")]
public class Drink_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Drink;
    }
}
