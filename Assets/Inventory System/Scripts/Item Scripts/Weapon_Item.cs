using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/New Weapon")]
public class Weapon_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Weapon;
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Weapons/" + name, out string localized))
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

        if (LocalizationManager.TryGetTranslation("Items/Weapons/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }
}
