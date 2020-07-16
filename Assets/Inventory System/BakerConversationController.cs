using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BakerInventory))]
public class BakerConversationController : ConversationController
{
    protected readonly string[] greetingsDialogues = new string[]
    {
        "Hello there!",
        "Hello, darling.",
        "What can I do for you?",
        "Nice to see you."
    };

    private BakerInventory bakerInventory;

    private void Start()
    {
        bakerInventory = GetComponent<BakerInventory>();
    }

    private int GetRandomDialogue(int count)
    {
        return Random.Range(0, count);
    }

    public override IEnumerator ConversationBegan()
    {
        answerOptionA.onClick.AddListener(bakerInventory.OpenShop);
        answerOptionA.onClick.AddListener(CloseShopConversation);
        answerOptionAText.text = "Open Shop.";
        answerOptionB.gameObject.SetActive(false);
        answerOptionC.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f); //Delay from camera transition

        conversationPanel.SetActive(true);
        characterDialogue.text = greetingsDialogues[GetRandomDialogue(greetingsDialogues.Length)];
    }

    public void CloseShopConversation()
    {
        ClearAllListeners();
        answerOptionAText.text = "Close Shop (Exits Conversation).";
        answerOptionA.onClick.AddListener(this.GetComponent<InteractableBaker>().OnDeFocus);
        answerOptionA.onClick.AddListener(PlayerManager.Instance.playerController.RemoveFocus);
    }

    public override void ConversationEnded()
    {
        conversationPanel.SetActive(false);
        ClearAllListeners();
        bakerInventory.CloseShop();
    }

    public void ClearAllListeners()
    {
        answerOptionA.onClick.RemoveAllListeners();
        answerOptionB.onClick.RemoveAllListeners();
        answerOptionC.onClick.RemoveAllListeners();
    }

    
}
