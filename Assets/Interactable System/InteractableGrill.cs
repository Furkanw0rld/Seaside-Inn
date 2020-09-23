using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGrill : Interactable
{
    private CookingWindowUI cookingWindowUI;
    private RecipeSystem recipeSystem;
    private DisplayMessageUI displayMessageUI;

    public GrillingArea grillingAreaLeft;
    public GrillingArea grillingAreaRight;

    public static InteractableGrill Instance;
    private InteractableStove interactableStove;

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
        interactableStove = InteractableStove.Instance;
    }

    public override void Interact()
    {
        HashSet<Recipe> grillRecipes = new HashSet<Recipe>();
        HashSet<Recipe> cookingRecipes = new HashSet<Recipe>();
        base.Interact();

        if (IsGrillingAreaAvailable())
        {
            grillRecipes = recipeSystem.GetAvailableRecipesForPlayer(true);
        }

        if (interactableStove.IsCookingAreaAvailable())
        {
            cookingRecipes = recipeSystem.GetAvailableRecipesForPlayer();
        }

        grillRecipes.UnionWith(cookingRecipes);

        if (grillRecipes.Count > 0)
        {
            cookingWindowUI.cookingWindow.SetActive(true);
            cookingWindowUI.DisplayRecipes(grillRecipes, StartGrilling);
        }
        else
        {
            if(!IsGrillingAreaAvailable() && !interactableStove.IsCookingAreaAvailable())
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

    public void StartGrilling(Recipe recipe)
    {
        if (!recipe.isGrilled)
        {
            interactableStove.StartCooking(recipe);
        }
        else
        {
            GrillingArea emptyArea = FindEmptyGrillingArea();
            StartCoroutine(emptyArea.FoodCooker(recipe));
        }
    }

    private GrillingArea FindEmptyGrillingArea()
    {
        if (!grillingAreaLeft.IsCooking && !grillingAreaLeft.IsRecipeHere) // Left is empty
        {
            return grillingAreaLeft;
        }

        if (!grillingAreaRight.IsCooking && !grillingAreaRight.IsRecipeHere) // Right is empty
        {
            return grillingAreaRight;
        }

        return null;
    }

    public bool IsGrillingAreaAvailable()
    {
        if (!grillingAreaLeft.IsCooking && !grillingAreaLeft.IsRecipeHere) // Left is empty
        {
            return true;
        }

        if(!grillingAreaRight.IsCooking && !grillingAreaRight.IsRecipeHere) // Right is empty
        {
            return true;
        }

        return false;
    }

}
