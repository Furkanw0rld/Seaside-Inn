using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; //UI Inventory Panel
    public ItemInformationWindowData informationWindow;
    [Header("Inventory Slots")]
    public Transform inventorySlots;
    public Transform innInventorySlots;
    public Transform tradeInventorySlots;

    [Header("UI Tab Buttons")]
    public Button playerInventoryButton;
    public Button innInventoryButton;
    public Button tradeInventoryButton;

    [Header("UI Sprites")]
    public Sprite genericIcon;
    public Sprite activeButtonSprite;
    public Sprite inactiveButtonSprite;

    private PlayerInventory playerInventory; //Cached inventory system
    private InventorySlot[] slots; // Player Inventory UI
    private InventorySlot[] innSlots; // Inn Inventory UI
    private InventorySlot[] tradeSlots; // Trade Inventory UI

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        playerInventory.onInventoryChangedCallback += UpdateUI;

        slots = inventorySlots.GetComponentsInChildren<InventorySlot>(); //Get all the inventory slots from scene
        innSlots = innInventorySlots.GetComponentsInChildren<InventorySlot>();
        tradeSlots = tradeInventorySlots.GetComponentsInChildren<InventorySlot>();

        foreach (InventorySlot slot in slots)
        {
            slot.amountText.enabled = false;
            slot.icon.enabled = false;
            slot.item = null;
            slot.amount = 0;
            slot.informationWindow = informationWindow;
            slot.inventoryType = InventoryType.PlayerInventory;
        }

        foreach (InventorySlot slot in innSlots)
        {
            slot.amountText.enabled = false;
            slot.icon.enabled = false;
            slot.item = null;
            slot.amount = 0;
            slot.informationWindow = informationWindow;
            slot.inventoryType = InventoryType.InnInventory;
        }

        foreach (InventorySlot slot in tradeSlots)
        {
            slot.amountText.enabled = false;
            slot.icon.enabled = false;
            slot.item = null;
            slot.amount = 0;
            slot.informationWindow = informationWindow;
            slot.inventoryType = InventoryType.TradeInventory;
        }


        //Link Tab Buttons to Inventory
        // Clear Buttons Listeners
        playerInventoryButton.onClick.RemoveAllListeners();
        innInventoryButton.onClick.RemoveAllListeners();
        tradeInventoryButton.onClick.RemoveAllListeners();
        //Add the controllers
        playerInventoryButton.onClick.AddListener(delegate { ChangeInventoryTabs(InventoryType.PlayerInventory); });
        innInventoryButton.onClick.AddListener(delegate { ChangeInventoryTabs(InventoryType.InnInventory); });
        tradeInventoryButton.onClick.AddListener(delegate { ChangeInventoryTabs(InventoryType.TradeInventory); });

        UpdateUI(InventoryType.InnInventory); //Update inn inventory, since there are starting items within it

    }

    private void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.activeSelf);
        }
    }

    private void OnDisable()
    {
        playerInventory.onInventoryChangedCallback -= UpdateUI;
    }

    private void UpdateUI(InventoryType inventoryType)
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory:
                for (int i = 0; i < slots.Length; i++)
                {
                    if (i < playerInventory.inventory.Count) //Items in inventory
                    {
                        DisplayItemAtSlot(i, inventoryType);
                    }
                    else
                    {
                        EmptyItemSlot(i, inventoryType);
                    }
                }
                break;

            case InventoryType.InnInventory:
                for(int i = 0; i < innSlots.Length; i++)
                {
                    if(i < playerInventory.innInventory.Count)
                    {
                        DisplayItemAtSlot(i, inventoryType);
                    }
                    else
                    {
                        EmptyItemSlot(i, inventoryType);
                    }
                }
                break;

            case InventoryType.TradeInventory:
                for(int i = 0; i < tradeSlots.Length; i++)
                {
                    if(i < playerInventory.tradeInventory.Count)
                    {
                        DisplayItemAtSlot(i, inventoryType);
                    }
                    else
                    {
                        EmptyItemSlot(i, inventoryType);
                    }
                }
                break;

            default:
                break;
        }


        Debug.Log("Inventory UI Updated. " + inventoryType.ToString());
    }

    private void DisplayItemAtSlot(int index, InventoryType inventoryType)
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory:
                slots[index].amountText.enabled = true;
                slots[index].icon.enabled = true;
                slots[index].item = playerInventory.inventory[index].item;
                slots[index].amount = playerInventory.inventory[index].amount;

                slots[index].amountText.text = playerInventory.inventory[index].amount.ToString();

                if (playerInventory.inventory[index].item.icon) // If the Item has an icon display it, otherwise show generic.
                {
                    slots[index].icon.sprite = playerInventory.inventory[index].item.icon;
                }
                else
                {
                    slots[index].icon.sprite = genericIcon;
                }

                break;

            case InventoryType.InnInventory:
                innSlots[index].amountText.enabled = true;
                innSlots[index].icon.enabled = true;
                innSlots[index].item = playerInventory.innInventory[index].item;
                innSlots[index].amount = playerInventory.innInventory[index].amount;

                innSlots[index].amountText.text = playerInventory.innInventory[index].amount.ToString();

                if (playerInventory.innInventory[index].item.icon)
                {
                    innSlots[index].icon.sprite = playerInventory.innInventory[index].item.icon;
                }
                else
                {
                    innSlots[index].icon.sprite = genericIcon;
                }

                break;

            case InventoryType.TradeInventory:
                tradeSlots[index].amountText.enabled = true;
                tradeSlots[index].icon.enabled = true;
                tradeSlots[index].item = playerInventory.tradeInventory[index].item;
                tradeSlots[index].amount = playerInventory.tradeInventory[index].amount;

                tradeSlots[index].amountText.text = playerInventory.tradeInventory[index].amount.ToString();

                if (playerInventory.tradeInventory[index].item.icon)
                {
                    tradeSlots[index].icon.sprite = playerInventory.tradeInventory[index].item.icon;
                }
                else
                {
                    tradeSlots[index].icon.sprite = genericIcon;
                }

                break;
        }

    }

    private void EmptyItemSlot(int index, InventoryType inventoryType)
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory:
                slots[index].amountText.enabled = false;
                slots[index].icon.enabled = false;
                slots[index].item = null;
                slots[index].amount = 0;
                break;

            case InventoryType.InnInventory:
                innSlots[index].amountText.enabled = false;
                innSlots[index].icon.enabled = false;
                innSlots[index].item = null;
                innSlots[index].amount = 0;
                break;

            case InventoryType.TradeInventory:
                tradeSlots[index].amountText.enabled = false;
                tradeSlots[index].icon.enabled = false;
                tradeSlots[index].item = null;
                tradeSlots[index].amount = 0;
                break;
        }

    }

    private void ChangeInventoryTabs(InventoryType inventoryType)
    {
        switch (inventoryType)
        {
            case InventoryType.PlayerInventory:
                // Change button sprite
                playerInventoryButton.image.sprite = activeButtonSprite;
                innInventoryButton.image.sprite = inactiveButtonSprite;
                tradeInventoryButton.image.sprite = inactiveButtonSprite;

                //Switch Active Tab:
                innInventorySlots.gameObject.SetActive(false);
                tradeInventorySlots.gameObject.SetActive(false);
                inventorySlots.gameObject.SetActive(true);
                break;

            case InventoryType.InnInventory:
                // Change button sprite
                playerInventoryButton.image.sprite = inactiveButtonSprite;
                innInventoryButton.image.sprite = activeButtonSprite;
                tradeInventoryButton.image.sprite = inactiveButtonSprite;

                //Switch Active Tab:
                tradeInventorySlots.gameObject.SetActive(false);
                inventorySlots.gameObject.SetActive(false);
                innInventorySlots.gameObject.SetActive(true);
                break;

            case InventoryType.TradeInventory:
                // Change button sprite
                playerInventoryButton.image.sprite = inactiveButtonSprite;
                innInventoryButton.image.sprite = inactiveButtonSprite;
                tradeInventoryButton.image.sprite = activeButtonSprite;

                //Switch Active Tab:
                inventorySlots.gameObject.SetActive(false);
                innInventorySlots.gameObject.SetActive(false);
                tradeInventorySlots.gameObject.SetActive(true);
                break;
        }
    }

}
