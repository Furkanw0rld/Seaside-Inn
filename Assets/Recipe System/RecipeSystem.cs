using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeSystem : MonoBehaviour
{
    private Recipe[] recipeArray; //Using this to get all the recipes
    private Food_Item[] foodArray; //Using to get all food items

    public Dictionary<string, HashSet<Recipe>> recipes = new Dictionary<string, HashSet<Recipe>>();

    public static RecipeSystem Instance;

    private PlayerInventory playerInventory;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        recipeArray = Resources.LoadAll<Recipe>("Recipe System/Recipes"); 
        foodArray = Resources.LoadAll<Food_Item>("Inventory System/Items/Foods"); 

        for(int i = 0; i < foodArray.Length; i++)
        {
            for(int j = 0; j < recipeArray.Length; j++)
            {
                Ingredient result = recipeArray[j].ingredients.FirstOrDefault(x => x.item.name == foodArray[i].name);

                if(result.item != null)
                {
                    if (!recipes.ContainsKey(foodArray[i].name))
                    {
                        recipes.Add(foodArray[i].name, new HashSet<Recipe>());
                    }

                    recipes[foodArray[i].name].Add(recipeArray[j]);
                }
                else
                {
                    continue;
                }
            }
        }

        playerInventory = PlayerInventory.Instance;
    }

    public HashSet<Recipe> GetAvailableRecipesForPlayer()
    {
        HashSet<Recipe> availableRecipes = new HashSet<Recipe>();

        for(int i = 0; i < playerInventory.inventoryFoodTracker.Count; i++)
        {
            if (recipes.ContainsKey(playerInventory.inventoryFoodTracker.ElementAt(i).Key)) //if recipes contains a key for this item, that means there are recipes available
            {
                string currentKey = playerInventory.inventoryFoodTracker.ElementAt(i).Key;

                for(int j = 0; j < recipes[currentKey].Count; j++) // Go through the recipes in the current key
                {
                    if (availableRecipes.Contains(recipes[currentKey].ElementAt(j))) //Recipe is already in the hashset skip
                    {
                        continue;
                    }
                    else
                    {

                        List<Ingredient> ingredientList = recipes[currentKey].ElementAt(j).ingredients;
                        for (int k = 0; k < ingredientList.Count; k++) // Go through it's ingredients list, and make sure that the player has atleast that amount
                        {
                            if (playerInventory.inventoryFoodTracker.ContainsKey(ingredientList[k].item.name)) //We do have the item, make sure the amount is greater than the ingredient requirement
                            {
                                if (playerInventory.inventoryFoodTracker[ingredientList[k].item.name] < ingredientList[k].amount) //If we don't have enough of the item break and go to next recipe
                                {
                                    break;
                                }
                                
                                if (k == ingredientList.Count - 1) //Last item in the list
                                {
                                    availableRecipes.Add(recipes[currentKey].ElementAt(j)); //Add to available recipes and break
                                    break;
                                }
                            }
                            else // We don't have the item, break right away and continue to next recipe
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        return availableRecipes;
    }

}
