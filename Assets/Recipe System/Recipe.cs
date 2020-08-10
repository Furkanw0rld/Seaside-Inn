using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipes/New Recipe", order = -1)]
public class Recipe : ScriptableObject
{
    [Tooltip("Name of the Recipe")] public new string name;
    [Tooltip("Icon of the Recipe")] public Sprite icon;
    [Tooltip("How many uses does the recipe have")] [Min(1)] public int amountOfUses;
    [Tooltip("How long does the food take to cook (in seconds)")] [Min(10)] public float cookTime;

    [Header("Models")] [Tooltip("Serving Model")] public GameObject model;
    [Tooltip("Cooking Model")] public GameObject cookingModel;
    [Tooltip("Cooked Model")] public GameObject cookedModel;
    
    [Space] [Tooltip("The Items needed for the recipe")] public List<Ingredient> ingredients;
    
}

[System.Serializable]
public struct Ingredient
{
    [Tooltip("Name of the item required")] public Food_Item item;
    [Tooltip("Amount needed of the item")] [Min(1)] public int amount;
}
