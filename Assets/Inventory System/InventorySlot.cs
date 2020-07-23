using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon; //Reference to UI Slot
    public TextMeshProUGUI amountText; //Reference to amount text
    public Image overlayImage;
    [HideInInspector] public Item item; //Reference to the current item in slot passed through inventory.
    [HideInInspector] public int amount;
    [HideInInspector] public ItemInformationWindowData informationWindow;
    [HideInInspector] public InventoryType inventoryType;

    private PlayerManager playerManager; //Cached PlayerManager Instance
    private PlayerInventory playerInventory;

    private void Start()
    {
        if (PlayerManager.Instance)
        {
            playerManager = PlayerManager.Instance;
        }

        if (PlayerInventory.Instance)
        {
            playerInventory = PlayerInventory.Instance;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item)
        {
            informationWindow.gameObject.SetActive(true);
            informationWindow.gameObject.transform.position = this.transform.position;
            informationWindow.itemName.text = item.GetLocalizedName();
            informationWindow.itemName.color = item.GetQualityColor();
            informationWindow.itemIcon.sprite = item.icon;
            informationWindow.itemDescription.text = item.GetLocalizedDescription();
            informationWindow.itemPrice.text = item.itemPrice.ToString();
            informationWindow.itemQuality.text = item.GetQuality().ToUpper();
            informationWindow.itemQuality.color = item.GetQualityColor();
            informationWindow.itemAmountLimit.text = amountText.text + "/" + item.itemAmountLimit.ToString();
            ShowItemMoveableOverlay();
        }
        else
        {
            informationWindow.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        informationWindow.gameObject.SetActive(false);
        overlayImage.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if(playerManager.isPlayerAtInnInventoryArea || playerManager.isPlayerAtTradeInventoryArea) //Trigger only when player is in these zones, (Move Item will do it's own safety check)
                {
                    MoveItem();
                }
            }
        }

    }

    private void MoveItem()
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory: //Item is in player, Can move to inn or trade

                if (playerManager.isPlayerAtInnInventoryArea) // Can move to inn
                {
                    if(item.itemType == ItemType.Food) //Only accept food
                    {
                        playerInventory.MoveItem(item, amount, InventoryType.InnInventory);
                        overlayImage.enabled = false;
                    }
                }
                else if (playerManager.isPlayerAtTradeInventoryArea)
                {
                    if(item.itemType != ItemType.Food && item.itemType != ItemType.Drink)
                    {
                        playerInventory.MoveItem(item, amount, InventoryType.TradeInventory);
                        overlayImage.enabled = false;
                    }
                }
                else
                {
                    break;
                }

                break;

            case InventoryType.InnInventory: //Item in inventory, Can move to player

                if(playerManager.isPlayerAtInnInventoryArea) //Can only move food out
                {
                    if (item.itemType == ItemType.Food) //Player is at the zone to move
                    {
                        playerInventory.MoveItem(item, amount, InventoryType.PlayerInventory);
                        overlayImage.enabled = false;

                    }
                }
                else
                {
                    return;
                }

                break;

            case InventoryType.TradeInventory: //Item in trade, Can move to player
                if (playerManager.isPlayerAtTradeInventoryArea)
                {
                    if(item.itemType != ItemType.Food)
                    {
                        playerInventory.MoveItem(item, amount, InventoryType.PlayerInventory);
                        overlayImage.enabled = false;
                    }
                    else
                    {
                        return;
                    }
                }
                break;

        }
    }

    private void ShowItemMoveableOverlay()
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory:
                if (playerManager.isPlayerAtInnInventoryArea) 
                {
                    // Player is at Inn, move to inn items
                    if(item.itemType == ItemType.Food) // Only need to check food since drinks are automatically added
                    {
                        overlayImage.enabled = true;
                    }
                    else
                    {
                        overlayImage.enabled = false;
                    }

                }
                else if (playerManager.isPlayerAtTradeInventoryArea)
                {
                    //Player at Trade Area, move to trade area items
                    if(item.itemType != ItemType.Food) // Accept anything but food/drink
                    {
                        overlayImage.enabled = true;

                    }
                    else
                    {
                        overlayImage.enabled = false;
                    }

                }
                else
                {
                    overlayImage.enabled = false;
                }
                break;

            case InventoryType.InnInventory:
                if (playerManager.isPlayerAtInnInventoryArea)
                {
                    if(item.itemType == ItemType.Food)
                    {
                        overlayImage.enabled = true;
                    }
                    else
                    {
                        overlayImage.enabled = false;
                    }
                }
                break;

            case InventoryType.TradeInventory:
                if (playerManager.isPlayerAtTradeInventoryArea)
                {
                    if (item.itemType != ItemType.Food) // Accept anything but food/drink
                    {
                        overlayImage.enabled = true;

                    }
                    else
                    {
                        overlayImage.enabled = false;
                    }
                }
                break;
        }

    }

}
