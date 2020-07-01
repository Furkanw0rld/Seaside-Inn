using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/New Weapon")]
public class Weapon_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Weapon;
    }
}
