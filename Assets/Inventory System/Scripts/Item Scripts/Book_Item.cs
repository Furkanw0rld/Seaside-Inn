using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Book", menuName = "Inventory/New Book", order = 1)]
public class Book_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Book;
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Books/" + name, out string localized))
        {
            return localized;
        }
        else
        {
            return name;
        }
    }

    public override string GetLocalizedDescription()
    {

        if (LocalizationManager.TryGetTranslation("Items/Books/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }
}
