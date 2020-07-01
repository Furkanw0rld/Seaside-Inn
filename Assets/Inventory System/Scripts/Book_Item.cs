using UnityEngine;

[CreateAssetMenu(fileName = "New Book", menuName = "Inventory/New Book")]
public class Book_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Book;
    }
}
