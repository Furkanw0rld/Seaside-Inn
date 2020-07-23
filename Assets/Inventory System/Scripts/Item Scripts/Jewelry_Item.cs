using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Jewelry", menuName = "Inventory/New Jewelry")]
public class Jewelry_Item : Item
{
    private void Awake()
    {
        itemType = ItemType.Jewelry;
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Jewelry/" + name, out string localized))
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

        if (LocalizationManager.TryGetTranslation("Items/Jewelry/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }
}
