using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookingWindowUI : MonoBehaviour
{
    public GameObject cookingWindow;
    public GameObject contentWindow;
    public GameObject recipeSlot;

    public static CookingWindowUI Instance;

    public List<GameObject> slotItems;

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
        if (cookingWindow.activeSelf)
        {
            cookingWindow.SetActive(false);
        }

        playerInventory = PlayerInventory.Instance;
    }

    public void DisplayRecipes(HashSet<Recipe> recipesToDisplay)
    {
        ClearContentWindow();

        foreach(Recipe rp in recipesToDisplay)
        {
            GameObject slotItem = Instantiate(recipeSlot, contentWindow.transform);
            slotItems.Add(slotItem);

            RecipeSlot rpSlot = slotItem.GetComponent<RecipeSlot>();

            rpSlot.recipeName.text = rp.name;
            rpSlot.recipeImage.sprite = rp.icon;
            rpSlot.amountOfUses.text = rp.amountOfUses.ToString();
            foreach(Ingredient ing in rp.ingredients)
            {
                TextMeshProUGUI ingredientText = Instantiate(rpSlot.ingredientText, rpSlot.ingredientsContentWindow.transform);
                ingredientText.text = "- " + ing.amount + " " + ing.item.name;
                ingredientText.gameObject.SetActive(true);
            }
            rpSlot.cookButton.onClick.AddListener(delegate { CookRecipe(rp); });
        }

        string temp = "Items in Dict \n";
        foreach (KeyValuePair<string, int> kp in playerInventory.inventoryFoodTracker)
        {
            temp += "- " + kp.Key + " contains: " + kp.Value + "\n";
        }

        Debug.Log(temp);
    }

    private void ClearContentWindow()
    {
        foreach(GameObject go in slotItems)
        {
            Destroy(go);
        }
        slotItems.Clear();
    }

    private void CookRecipe(Recipe recipe)
    {
        for(int i = 0; i < recipe.ingredients.Count; i++)
        {
            playerInventory.FindAndUseFoodItem(recipe.ingredients[i].item, recipe.ingredients[i].amount);
        }
        PlayerManager.Instance.playerController.RemoveFocus();
    }
}
