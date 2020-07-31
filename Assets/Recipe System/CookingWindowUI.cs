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

        }
    }

    private void ClearContentWindow()
    {
        foreach(GameObject go in slotItems)
        {
            Destroy(go);
        }
        slotItems.Clear();
    }
}
