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
    [TextArea(2, 10)] public string description;
    [Tooltip("Model of the item.")] public GameObject prefab;
    [Tooltip("Price of the item.")] [Min(0f)] public float itemPrice = 0; 
    [Tooltip("Quality of the item.")] public ItemQuality itemQuality;

}

