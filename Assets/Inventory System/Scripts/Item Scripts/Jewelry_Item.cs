using UnityEngine;

[CreateAssetMenu(fileName = "New Jewelry", menuName = "Inventory/New Jewelry")]
public class Jewelry_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Jewelry;
    }
}
