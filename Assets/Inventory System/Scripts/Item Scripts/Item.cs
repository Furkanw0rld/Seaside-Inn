using UnityEngine;

public enum ItemType
{
    Food,
    Drink,
    Armor,
    Weapon,
    Jewelry,
    Book
}

public enum ItemQuality
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public abstract class Item : ScriptableObject
{
    [Header("Item Details")]
    [Tooltip("Name of the Item.")] new public string name = "New Item";
    [Tooltip("Type of Item.")] [ReadOnly] public ItemType itemType;
    [Tooltip("Icon shown in player inventory.")] public Sprite icon = null;
    [TextArea(2, 3)] public string description;
    [Tooltip("Model of the item.")] public GameObject prefab;
    [Tooltip("Price of the item.")] [Min(0f)] public float itemPrice = 0; 
    [Tooltip("Quality of the item.")] public ItemQuality itemQuality;
    [Tooltip("How many should be allowed in the players inventory?")] [Min(1)] public int itemAmountLimit = 5;

    public virtual string GetQuality()
    {
        return itemQuality.ToString();
    }

    public virtual Color32 GetQualityColor()
    {
        switch (itemQuality)
        {
            case ItemQuality.Common:
                return new Color32(187, 190, 195, 255); //Gray
            case ItemQuality.Uncommon:
                return new Color32(39, 190, 42, 255); //Green
            case ItemQuality.Rare:
                return new Color32(10, 89, 221, 255); //Blue
            case ItemQuality.Epic:
                return new Color32(170, 16, 176, 255); //Purple
            case ItemQuality.Legendary:
                return new Color32(235, 151, 18, 255); // Orange
            default:
                return new Color32(187, 190, 195, 255); // common color.
        }
    }
}

