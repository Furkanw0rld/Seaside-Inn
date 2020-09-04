using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStove : Interactable
{
    private CookingWindowUI cookingWindowUI;
    private RecipeSystem recipeSystem;

    public CookingArea cookingAreaRight;
    public CookingArea cookingAreaCenter;
    public CookingArea cookingAreaLeft;

    public static InteractableStove Instance;

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

    private new void Start()
    {
        cookingWindowUI = CookingWindowUI.Instance;
        recipeSystem = RecipeSystem.Instance;
    }

    public override void Interact()
    {
        base.Interact();

        CookingArea getEmptyArea = FindEmptyCookingArea();
        if(getEmptyArea)
        {
            cookingWindowUI.cookingWindow.SetActive(true);
            cookingWindowUI.DisplayRecipes(recipeSystem.GetAvailableRecipesForPlayer(), StartCooking);
        }
    }

    public override void OnDeFocus()
    {
        cookingWindowUI.cookingWindow.SetActive(false);
        base.OnDeFocus();
    }

    public void StartCooking(Recipe recipe)
    {
        CookingArea emptyArea = FindEmptyCookingArea();
        Debug.Log("We began cooking: " + recipe.name);
        StartCoroutine(emptyArea.FoodCooker(recipe));
    }

    private CookingArea FindEmptyCookingArea()
    {
        if (!cookingAreaRight.IsCooking && !cookingAreaRight.IsRecipeHere) // Right is empty
        {
            return cookingAreaRight;
        }

        if (!cookingAreaCenter.IsCooking && !cookingAreaCenter.IsRecipeHere) // Center is empty
        {
            return cookingAreaCenter;
        }

        if (!cookingAreaLeft.IsCooking && !cookingAreaLeft.IsRecipeHere) // Left is empty
        {
            return cookingAreaLeft;
        }

        return null;
    }

}
