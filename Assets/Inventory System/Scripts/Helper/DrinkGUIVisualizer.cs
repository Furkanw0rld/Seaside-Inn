using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class DrinkGUIVisualizer : MonoBehaviour
{
    [SerializeField] private DrinkName drinkName = DrinkName.Ale;
#pragma warning disable 0649
    [SerializeField] private GameObject informationWindow;
    [SerializeField] private Image radialBar;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Gradient barGradient;
#pragma warning restore 0649

    private InventorySlotItem drinkSlot;
    private int amountDisplayed = 0;

    void Start()
    {
        if (PlayerInventory.Instance)
        {
            drinkSlot = PlayerInventory.Instance.innInventory[(int)drinkName];
        }

        if (informationWindow.activeSelf)
        {
            informationWindow.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        amountDisplayed = drinkSlot.amount;

        if (amountDisplayed > 1)
        {
            
            amountText.text = amountDisplayed + " " + I2.Loc.LocalizationManager.GetTranslation("Pints");
        }
        else
        {
            amountText.text = amountDisplayed + " " + I2.Loc.LocalizationManager.GetTranslation("Pint");
        }

        radialBar.fillAmount = amountDisplayed / (float)drinkSlot.item.itemAmountLimit;
        radialBar.color = barGradient.Evaluate(radialBar.fillAmount);

        informationWindow.SetActive(true);
    }

    private void OnMouseOver()
    {
        informationWindow.transform.rotation = Quaternion.LookRotation(informationWindow.transform.position - Camera.main.transform.position);

        if(drinkSlot.amount != amountDisplayed) // A change has occurred
        {
            amountDisplayed = drinkSlot.amount;

            if (amountDisplayed > 1)
            {
                amountText.text = amountDisplayed + " " + I2.Loc.LocalizationManager.GetTranslation("Pints");
            }
            else
            {
                amountText.text = amountDisplayed + " " + I2.Loc.LocalizationManager.GetTranslation("Pint");
            }

            radialBar.fillAmount = amountDisplayed / (float)drinkSlot.item.itemAmountLimit;
            radialBar.color = barGradient.Evaluate(radialBar.fillAmount);
        }

    }

    private void OnMouseExit()
    {
        informationWindow.SetActive(false);
    }
}


