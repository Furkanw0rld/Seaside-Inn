using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/New Armor")]
public class Armor_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Armor;
    }

}
