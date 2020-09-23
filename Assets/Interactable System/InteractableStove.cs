using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStove : Interactable
{
    private CookingWindowUI cookingWindowUI;
    private RecipeSystem recipeSystem;
    private DisplayMessageUI displayMessageUI;

    public CookingArea cookingAreaRight;
    public CookingArea cookingAreaCenter;
    public CookingArea cookingAreaLeft;

    public static InteractableStove Instance;
    private InteractableGrill interactableGrill;

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
        displayMessageUI = DisplayMessageUI.Instance;
        interactableGrill = InteractableGrill.Instance;
    }

    public override void Interact()
    {
        HashSet<Recipe> grillRecipes = new HashSet<Recipe>();
        HashSet<Recipe> availableRecipes = new HashSet<Recipe>();
        base.Interact();

        if (IsCookingAreaAvailable())
        {
            availableRecipes = recipeSystem.GetAvailableRecipesForPlayer();
        }

        if (interactableGrill.IsGrillingAreaAvailable())
        {
            grillRecipes = recipeSystem.GetAvailableRecipesForPlayer(true);
        }

        availableRecipes.UnionWith(grillRecipes);

        if(availableRecipes.Count > 0)
        {
            cookingWindowUI.cookingWindow.SetActive(true);
            cookingWindowUI.DisplayRecipes(availableRecipes, StartCooking);
        }
        else
        {
            if (!interactableGrill.IsGrillingAreaAvailable() && !IsCookingAreaAvailable())
            {
                displayMessageUI.DisplayMessage("There aren't any available cooking pits.");
            }
            else
            {
                displayMessageUI.DisplayMessage("There aren't any available <color=#ffde00>recipes</color>.");
            }

        }

    }

    public override void OnDeFocus()
    {
        cookingWindowUI.cookingWindow.SetActive(false);
        base.OnDeFocus();
    }

    public void StartCooking(Recipe recipe)
    {
        if (recipe.isGrilled)
        {
            interactableGrill.StartGrilling(recipe);
        }
        else
        {
            CookingArea emptyArea = FindEmptyCookingArea();
            StartCoroutine(emptyArea.FoodCooker(recipe));
        }
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

    public bool IsCookingAreaAvailable()
    {
        if (!cookingAreaRight.IsCooking && !cookingAreaRight.IsRecipeHere) // Right is empty
        {
            return true;
        }

        if (!cookingAreaCenter.IsCooking && !cookingAreaCenter.IsRecipeHere) // Center is empty
        {
            return true;
        }

        if (!cookingAreaLeft.IsCooking && !cookingAreaLeft.IsRecipeHere) // Left is empty
        {
            return true;
        }

        return false;
    }

}
