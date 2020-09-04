using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class CookingArea : Interactable
{
    private const float ALLOWED_MIN_RATIO = 0.9f; // The percent we are allowed to undercook by, this becomes the minimum allowed time (cookTime of 30 -> 27) the food is considered cooked after 27seconds
    private const float ALLOWED_MAX_RATIO = 1.35f; // The percent we are allowed to overcook by, this amount gets added to cooktimer (cookTime of 30 -> 40.5) we overcook after 40.5seconds (Food is burnt)

    public GameObject cookingInformationWindow;
    [HideInInspector] public int usesLeft = 0;

    [Header("UI Information")]
#pragma warning disable 0649
    [SerializeField] private Gradient cookingGradient;
    [SerializeField] private Gradient usesLeftGradient;
    [SerializeField] private Image radialBar;
    [SerializeField] private TextMeshProUGUI usesLeftText;
    [Header("Fire Effects")]
    [SerializeField] private ParticleSystem fireEffect;
#pragma warning restore 0649

    [Tooltip("Current recipe at this cooking area.")] private Recipe currentRecipe;
    private BoxCollider collisionMesh; //The Mesh Area we interact with

    private GameObject cookingGameObject;
    private GameObject cookedGameObject;

    private float cookTimer = 0;

    public bool IsCooking { get; set; } // Is there something actively cooking
    public bool IsRecipeHere { get; set; } // Is there a recipe here that's being used

    private void Awake()
    {
        collisionMesh = this.GetComponent<BoxCollider>();
        collisionMesh.enabled = false;
        cookingInformationWindow.SetActive(false);

        //fireEffect.transform.localPosition = new Vector3(0, fireEffect.transform.localPosition.y - 0.4f, 0);
        fireEffect.Stop();
    }

    public IEnumerator FoodCooker(Recipe recipe)
    {
        currentRecipe = recipe;
        IsCooking = true;
        usesLeft = currentRecipe.amountOfUses;
        collisionMesh.enabled = true;
        cookingGameObject = Instantiate(currentRecipe.cookingModel, this.transform);
        cookingInformationWindow.SetActive(true);
        usesLeftText.gameObject.SetActive(false);
        StartCoroutine(ManageCookTimer());

        fireEffect.Play();
        yield return null;
    }

    IEnumerator ManageCookTimer()
    {
        cookTimer = 0f;

        while (IsCooking)
        {
            radialBar.fillAmount = cookTimer / currentRecipe.cookTime;
            radialBar.color = cookingGradient.Evaluate(cookTimer / (currentRecipe.cookTime * ALLOWED_MAX_RATIO));
            cookTimer += Time.deltaTime;
            yield return null;
        }
    }

    public override void Interact()
    {
        if (!IsRecipeHere && IsCooking) // We don't have an active recipe that's being used, that means we were cooking.
        {
            IsCooking = false;
            float allowedMinimumTime = currentRecipe.cookTime * ALLOWED_MIN_RATIO;
            float allowedMaximumTime = currentRecipe.cookTime * ALLOWED_MAX_RATIO;

            if (cookTimer >= allowedMinimumTime && cookTimer <= allowedMaximumTime) // We are in the range, where the food is cooked. Use.
            {
                Debug.Log("Food is cooked!");
                IsRecipeHere = true;
                cookedGameObject = Instantiate(currentRecipe.cookedModel, this.transform);
                collisionMesh.enabled = true;
                Destroy(cookingGameObject);
                cookingGameObject = null;

                usesLeft = currentRecipe.amountOfUses;
                usesLeftText.gameObject.SetActive(true);
                usesLeftText.text = usesLeft + "/" + currentRecipe.amountOfUses;
                radialBar.color = usesLeftGradient.Evaluate((float) usesLeft / currentRecipe.amountOfUses);
                radialBar.fillAmount = 1f;
                fireEffect.Stop();
            }
            else // The food is either overcooked or undercooked. Destroy.
            {
                Debug.Log("Food isn't cooked! Trashing.");
                IsRecipeHere = false;
                usesLeft = 0;
                collisionMesh.enabled = false;
                currentRecipe = null;
                cookingInformationWindow.SetActive(false);
                Destroy(cookingGameObject);
                cookingGameObject = null;
                fireEffect.Stop();
            }
        }
        else if (IsRecipeHere && !IsCooking && usesLeft > 0)
        {
            usesLeft--;
            usesLeftText.text = usesLeft + "/" + currentRecipe.amountOfUses;
            radialBar.color = usesLeftGradient.Evaluate((float) usesLeft / currentRecipe.amountOfUses);
            radialBar.fillAmount = (float)usesLeft / currentRecipe.amountOfUses;
            Debug.Log("We used 1 " + currentRecipe.name);
            // TODO: Use the food here

            if (usesLeft == 0) // Now that an item is used, we should check if there any uses left, if there isn't empty this slot.
            {
                IsRecipeHere = false;
                IsCooking = false;
                collisionMesh.enabled = false;
                currentRecipe = null;
                Destroy(cookedGameObject);
                cookedGameObject = null;
                cookingGameObject = null;
                cookingInformationWindow.SetActive(false);
            }
        }

        base.Interact();
    }
}
