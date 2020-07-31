using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/New Armor", order = 1)]
public class Armor_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Armor;
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Armors/" + name, out string localized))
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

        if (LocalizationManager.TryGetTranslation("Items/Armors/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }

}
