using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConversationControllerBrewer : ConversationController
{
    private InventoryBrewer inventoryBrewer;
    private InteractableBrewer interactableBrewer;
    private void Start()
    {
        conversationUI = ConversationUI.Instance;
        inventoryBrewer = GetComponent<InventoryBrewer>();
        interactableBrewer = GetComponent<InteractableBrewer>();
    }

    public override void ConversationBegan()
    {
        string dialogue = "What can I do for you?";
        string openShopDialogue = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/OpenShop");
        UnityAction[] answerACallbacks = new UnityAction[] { inventoryBrewer.OpenShop, CloseShopConversation };
        StartCoroutine(conversationUI.ShowConversationUI(interactableBrewer, dialogue, openShopDialogue, answerACallbacks));
    }
    public void CloseShopConversation()
    {
        string optionAText = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/CloseShop");
        UnityAction[] answerACallbacks = new UnityAction[] { interactableBrewer.OnDeFocus, PlayerManager.Instance.playerController.RemoveFocus };
        StartCoroutine(conversationUI.ShowConversationUI(interactableBrewer, "I've got all the drinks you need.", optionAText, answerACallbacks));
    }

    public override void ConversationEnded()
    {
        conversationUI.CloseConversationUI();
        inventoryBrewer.CloseShop();
    }
}
