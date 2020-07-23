using UnityEngine;
using I2.Loc;

//[CreateAssetMenu(fileName = "New Drink", menuName = "Inventory/New Drink")]
public class Drink_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Drink;
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Drinks/" + name, out string localized))
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
        if (LocalizationManager.TryGetTranslation("Items/Drinks/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }
}
