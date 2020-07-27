using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class DrinkGUIVisualizer : MonoBehaviour
{
    public DrinkName drinkName = DrinkName.Ale;
    public GameObject informationWindow;
    public Image radialBar;
    public TextMeshProUGUI amountText;
    public Gradient barGradient;

    private InventorySlotItem drinkSlot;

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
        
        if(drinkSlot.amount > 1)
        {
            amountText.text = drinkSlot.amount + I2.Loc.LocalizationManager.GetTranslation("Pints");
        }
        else
        {
            amountText.text = drinkSlot.amount + I2.Loc.LocalizationManager.GetTranslation("Pint");
        }

        radialBar.fillAmount = drinkSlot.amount / (float)drinkSlot.item.itemAmountLimit;
        radialBar.color = barGradient.Evaluate(radialBar.fillAmount);

        informationWindow.SetActive(true);
    }

    private void OnMouseOver()
    {
        informationWindow.transform.rotation = Quaternion.LookRotation(informationWindow.transform.position - Camera.main.transform.position);
    }

    private void OnMouseExit()
    {
        informationWindow.SetActive(false);
    }

}

public enum DrinkName //These IDs Follow the Drink's Position in the Inn Inventory. 
{ 
    Ale = 0,
    Cider = 1,
    Mead = 2,
    Wine = 3
}
