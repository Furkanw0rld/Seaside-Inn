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
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [Header("Transforms")]
    [SerializeField] private Transform recipeInstantiationPoint;
    [Header("Effect Systems")]
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private ParticleSystem bubblesEffect;
#pragma warning restore 0649

    [Tooltip("Current recipe at this cooking area.")] private Recipe currentRecipe;
    private BoxCollider collisionMesh; //The Mesh Area we interact with

    private GameObject cookingGameObject;

    private float cookTimer = 0;

    private readonly float recipeFullHeight = 0.64f;
    private readonly float recipeEmptyHeight = 0.5f;

    //Display Message Cache
    private DisplayMessageUI displayMessageUI;

    public bool IsCooking { get; set; } // Is there something actively cooking
    public bool IsRecipeHere { get; set; } // Is there a recipe here that's being used

    private void Awake()
    {
        collisionMesh = this.GetComponent<BoxCollider>();
        collisionMesh.enabled = false;
        cookingInformationWindow.SetActive(false);

        fireEffect.Stop();
        bubblesEffect.Stop();
    }

    protected override void Start()
    {
        displayMessageUI = DisplayMessageUI.Instance;
        base.Start();
    }

    public IEnumerator FoodCooker(Recipe recipe)
    {
        currentRecipe = recipe;
        IsCooking = true;
        usesLeft = currentRecipe.amountOfUses;
        recipeNameText.text = currentRecipe.name;
        collisionMesh.enabled = true;
        cookingGameObject = Instantiate(currentRecipe.cookingModel, recipeInstantiationPoint);
        StartCoroutine(FoodFillingEffect());
        cookingInformationWindow.SetActive(true);
        usesLeftText.gameObject.SetActive(false);
        StartCoroutine(ManageCookTimer());

        fireEffect.Play();
        yield return null;
    }

    private IEnumerator FoodFillingEffect()
    {
        float fillTime = 1.5f;
        float time = 0f;
        float normalized;

        recipeInstantiationPoint.localPosition = new Vector3(0f, recipeEmptyHeight, 0f);
        Vector3 recipeStartHeight = new Vector3(0f, recipeEmptyHeight, 0f);
        Vector3 recipeFinalHeight = new Vector3(0f, recipeFullHeight, 0f);

        while(time < fillTime)
        {
            normalized = time / fillTime;
            normalized = normalized * normalized * (3f - 2f * normalized); //Smoothing Algorithm
            recipeInstantiationPoint.localPosition = Vector3.Lerp(recipeStartHeight, recipeFinalHeight, normalized);
            time += Time.deltaTime;
            yield return null;
        }

        recipeInstantiationPoint.localPosition = recipeFinalHeight;
        bubblesEffect.Play();
    }

    private IEnumerator FoodDecreasingEffect(bool completelyDeplete = false)
    {
        float emptyingTime = 0.5f;
        float time = 0;
        float normalized;

        float nextHeight;
        Vector3 recipeFinalHeight;
        Vector3 recipeStartHeight = recipeInstantiationPoint.localPosition;

        if (completelyDeplete)
        {
            recipeFinalHeight = new Vector3(0f, recipeEmptyHeight, 0f);
            emptyingTime = 1.5f;
        }
        else
        {
            nextHeight = recipeEmptyHeight + ((recipeFullHeight - recipeEmptyHeight) * ((float)usesLeft / currentRecipe.amountOfUses));
            recipeFinalHeight = new Vector3(0f, nextHeight, 0f);
        }

        while(time < emptyingTime)
        {
            normalized = time / emptyingTime;
            normalized = normalized * normalized * (3f - 2f * normalized); //Smoothing algorithm
            recipeInstantiationPoint.localPosition = Vector3.Lerp(recipeStartHeight, recipeFinalHeight, normalized);
            time += Time.deltaTime;
            yield return null;
        }

        recipeInstantiationPoint.localPosition = recipeFinalHeight;

        if (completelyDeplete) // Destroy and nullify cookingGameObject if it still exits when we completely deplete
        {
            if(cookingGameObject != null)
            {
                Destroy(cookingGameObject);
                cookingGameObject = null;
            }
        }
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
                // Food is Cooked
                string message = "<color=#ffde00>" + currentRecipe.name + "</color> is cooked!";
                displayMessageUI.DisplayMessage(message);

                IsRecipeHere = true;
                collisionMesh.enabled = true;

                usesLeft = currentRecipe.amountOfUses;
                usesLeftText.gameObject.SetActive(true);
                usesLeftText.text = usesLeft + "/" + currentRecipe.amountOfUses;
                radialBar.color = usesLeftGradient.Evaluate((float)usesLeft / currentRecipe.amountOfUses);
                radialBar.fillAmount = 1f;
                fireEffect.Stop();
                bubblesEffect.Stop();
            }
            else // The food is either overcooked or undercooked. Destroy.
            {
                // Food isn't cooked
                string message = "<color=#8f3d0c>" + currentRecipe.name + "</color>";
                if (cookTimer >= allowedMaximumTime) //Overcooked
                {
                    message += " is burnt!";
                }
                else //Undercooked
                {
                    message += " is undercooked!";
                }
                displayMessageUI.DisplayMessage(message);

                IsRecipeHere = false;
                usesLeft = 0;
                collisionMesh.enabled = false;
                currentRecipe = null;
                cookingInformationWindow.SetActive(false);

                //No need to nullify cookingGO because we will do so at the end of Effect
                //Destroy(cookingGameObject);
                //cookingGameObject = null;
                StartCoroutine(FoodDecreasingEffect(true));
                fireEffect.Stop();
                bubblesEffect.Stop();
            }
        }
        else if (IsRecipeHere && !IsCooking && usesLeft > 0)
        {
            usesLeft--;
            StartCoroutine(FoodDecreasingEffect());

            usesLeftText.text = usesLeft + "/" + currentRecipe.amountOfUses;
            radialBar.color = usesLeftGradient.Evaluate((float)usesLeft / currentRecipe.amountOfUses);
            radialBar.fillAmount = (float)usesLeft / currentRecipe.amountOfUses;
            //Debug.Log("We used 1 " + currentRecipe.name);
            // TODO: Use the food here

            if (usesLeft == 0) // Now that an item is used, we should check if there any uses left, if there isn't empty this slot.
            {
                string message = "<color=#ffde00>" + currentRecipe.name + "</color> is all used up!";
                displayMessageUI.DisplayMessage(message);
                IsRecipeHere = false;
                IsCooking = false;
                collisionMesh.enabled = false;
                currentRecipe = null;
                Destroy(cookingGameObject);
                cookingGameObject = null;
                cookingInformationWindow.SetActive(false);
            }
        }

        base.Interact();
    }
}
