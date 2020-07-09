using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory/New Food")]
public class Food_Item : Item
{
    [Header("Food Item Related")]
    [Range(0f,5f)][Tooltip("0 implies no freshness decay.\n 1 implies 1 freshness decay per day.\n 2 implies 2 freshness decay per day.\n 5 implies food becomes spoilt the next day.")] public float FoodDecayModifier = 1f;
    [Tooltip("Starting Freshness for the item.")] public FoodFreshness freshness = FoodFreshness.VeryFresh;
    private void Awake()
    {
        itemType = ItemType.Food;
    }
}

public enum FoodFreshness
{
    VeryFresh,
    Fresh,
    Edible,
    Stale,
    Spoilt
}
