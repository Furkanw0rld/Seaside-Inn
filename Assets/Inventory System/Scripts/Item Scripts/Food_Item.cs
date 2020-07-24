using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory/New Food")]
public class Food_Item : Item
{
    [Header("Food Item Related")]
    [Range(0f,5f)][Tooltip("0 implies no freshness decay.\n 1 implies 1 freshness decay per day.\n 2 implies 2 freshness decay per day.\n 5 implies food becomes spoilt the next day. " +
        "\n Values between 0-1 imply decay that takes longer than 1 day.\n For Example: 0.5f means food decays every other day or 0.25 implies every 4 days to go down one tier. ")] public float FoodDecayModifier = 1f;
    [Tooltip("Starting Freshness for the item.")] public FoodFreshness freshness = FoodFreshness.VeryFresh;

    [HideInInspector] public float currentDecay = 0f;

    private void Awake()
    {
        itemType = ItemType.Food;
        if (DayNightCycle.Instance)
        {
            DayNightCycle.Instance.onDayTimeCallback += UpdateDecay;
        }
    }

    public void UpdateDecay()
    {
        currentDecay += FoodDecayModifier;
        DecayQuality();

        if(freshness == FoodFreshness.Spoilt)
        {
            PlayerInventory.Instance.RemoveSpoiltFoodFromInventory();
        }
    }
    public void OnDestroy()
    {
        DayNightCycle.Instance.onDayTimeCallback -= UpdateDecay;
    }

    private void DecayQuality() {
        switch (Mathf.FloorToInt(currentDecay))
        {
            case (0):
                freshness = FoodFreshness.VeryFresh;
                break;
            case (1):
                freshness = FoodFreshness.Fresh;
                break;
            case (2):
                freshness = FoodFreshness.Edible;
                break;
            case (3):
                freshness = FoodFreshness.Stale;
                break;
            case (4):
                freshness = FoodFreshness.Spoilt;
                break;
            default:
                if(currentDecay >= 4)
                {
                    freshness = FoodFreshness.Spoilt;
                }
                break;
        }
    }
    public override string GetQuality()
    {
        return LocalizationManager.GetTranslation("Items/Foods/Quality/" + freshness.ToString());
    }

    public override string GetLocalizedName()
    {
        if (LocalizationManager.TryGetTranslation("Items/Foods/" + name, out string localized))
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

        if (LocalizationManager.TryGetTranslation("Items/Foods/" + name + "/Description", out string localized))
        {
            return localized;
        }
        else
        {
            return description;
        }
    }

    public override Color32 GetQualityColor()
    {
        switch (freshness)
        {
            case FoodFreshness.VeryFresh:
                return new Color32(72, 208, 24, 255); //Very Green
            case FoodFreshness.Fresh:
                return new Color32(61, 170, 22, 255); //Green
            case FoodFreshness.Edible:
                return new Color32(47, 114, 23, 255); //Deep Green
            case FoodFreshness.Stale:
                return new Color32(150, 142, 15, 255); //Musty Yellow
            case FoodFreshness.Spoilt:
                return new Color32(143, 61, 12, 255); // Redish Orange
            default:
                return new Color32(72, 208, 24, 255); //Very Green
        }
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
