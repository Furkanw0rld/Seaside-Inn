using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStove : Interactable
{
    private CookingWindowUI cookingWindowUI;
    private RecipeSystem recipeSystem;

    private new void Start()
    {
        cookingWindowUI = CookingWindowUI.Instance;
        recipeSystem = RecipeSystem.Instance;
    }

    public override void Interact()
    {
        cookingWindowUI.cookingWindow.SetActive(true);
        base.Interact();

        cookingWindowUI.DisplayRecipes(recipeSystem.GetAvailableRecipesForPlayer());
    }

    public override void OnDeFocus()
    {
        cookingWindowUI.cookingWindow.SetActive(false);
        base.OnDeFocus();
    }

}
