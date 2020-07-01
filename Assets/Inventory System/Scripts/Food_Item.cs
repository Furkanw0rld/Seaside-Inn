using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory/New Food")]
public class Food_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Food;
    }
}
