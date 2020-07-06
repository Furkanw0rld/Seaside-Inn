using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEngine.UI.Image icon; //Reference to UI Slot
    public TextMeshProUGUI amountText; //Reference to amount text
    [HideInInspector] public Item item; //Reference to the current item in slot passed through inventory.
    [HideInInspector] public ItemInformationWindow informationWindow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item)
        {
            informationWindow.gameObject.SetActive(true);
            informationWindow.gameObject.transform.position = Input.mousePosition;

            informationWindow.itemName.text = item.name;
            informationWindow.itemIcon.sprite = item.icon;
            informationWindow.itemDescription.text = item.description;
            informationWindow.itemPrice.text = item.itemPrice.ToString();
            informationWindow.itemQuality.text = item.itemQuality.ToString();
            informationWindow.itemAmountLimit.text = amountText.text + "/" + item.itemAmountLimit.ToString();
        }
        else
        {
            informationWindow.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        informationWindow.gameObject.SetActive(false);
    }
}
