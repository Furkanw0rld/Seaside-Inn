using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InventoryButcher), typeof(InteractableButcher))]
public class ConversationControllerButcher : ConversationController
{
    private InventoryButcher butcherInventory;
    private InteractableButcher interactableButcher;
    // Start is called before the first frame update
    void Start()
    {
        conversationUI = ConversationUI.Instance;
        butcherInventory = GetComponent<InventoryButcher>();
        interactableButcher = GetComponent<InteractableButcher>();
    }

    public override void ConversationBegan()
    {
        string dialogue = "Hi there!";
        string openShopDialogue = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/OpenShop");
        UnityAction[] answerACallbacks = new UnityAction[] { butcherInventory.OpenShop, CloseShopConversation };
        StartCoroutine(conversationUI.ShowConversationUI(interactableButcher, dialogue, openShopDialogue, answerACallbacks));
    }

    public void CloseShopConversation()
    {
        string optionAText = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/CloseShop");
        UnityAction[] answerACallbacks = new UnityAction[] { interactableButcher.OnDeFocus, PlayerManager.Instance.playerController.RemoveFocus };
        StartCoroutine(conversationUI.ShowConversationUI(interactableButcher, "I sell the freshest meats.", optionAText, answerACallbacks));
    }

    public override void ConversationEnded()
    {
        conversationUI.CloseConversationUI();
        butcherInventory.CloseShop();
    }
}
