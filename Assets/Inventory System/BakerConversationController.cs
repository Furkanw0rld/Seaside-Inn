using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BakerConversationController : ConversationController
{
    protected readonly string[] boughtBreadDialogues = new string[]
    {
        "One fresh bread coming right up.",
        "Sure, that'll be one coin.",
        "Here is your bread.",
        "If you've got the coin, I've got the bread.",
        "Fresh bread, straight out of the stone oven!"
    };

    protected readonly string[] notEnoughCoinDialogues = new string[]
    {
        "You've got no coin to buy bread, dear.",
        "You need coin to buy bread.",
        "Come back and find me when you've got coin.",
        "I do not give bread for free.",
        "Bread isn't free you know?"
    };

    protected readonly string[] greetingsDialogues = new string[]
    {
        "Hello there!",
        "Hello, darling.",
        "What can I do for you?",
        "Nice to see you."
    };


    public Item breadItem;

    public void BuyBreadItem()
    {
        if (PlayerInventory.Instance.BuyItem(breadItem, breadItem.itemPrice, 1))
        {
            characterDialogue.text = boughtBreadDialogues[GetRandomDialogue(boughtBreadDialogues.Length)];
            answerOptionB.gameObject.SetActive(true);
            answerOptionBText.text = "- Thank you. (Exit Conversation)";
            answerOptionB.onClick.AddListener(PlayerManager.Instance.playerController.RemoveFocus);
        }
        else
        {
            characterDialogue.text = notEnoughCoinDialogues[GetRandomDialogue(notEnoughCoinDialogues.Length)];
            answerOptionB.gameObject.SetActive(true);
            answerOptionBText.text = "- Thank you. (Exit Conversation)";
            answerOptionB.onClick.AddListener(PlayerManager.Instance.playerController.RemoveFocus);
        }
    }

    private int GetRandomDialogue(int count)
    {
        return Random.Range(0, count);
    }

    public override IEnumerator ConversationBegan()
    {
        answerOptionA.onClick.AddListener(BuyBreadItem);
        answerOptionB.gameObject.SetActive(false);
        answerOptionC.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f); //Delay from camera transition

        conversationPanel.SetActive(true);
        characterDialogue.text = greetingsDialogues[GetRandomDialogue(greetingsDialogues.Length)];
        //To be overwritten by inherited children
        Debug.Log("Conversation Began. Enumerator.");
    }

    public override void ConversationEnded()
    {
        conversationPanel.SetActive(false);
        ClearAllListeners();
        Debug.Log("Conversation Ended.");
    }

    public void ClearAllListeners()
    {
        answerOptionA.onClick.RemoveAllListeners();
        answerOptionB.onClick.RemoveAllListeners();
        answerOptionC.onClick.RemoveAllListeners();
    }
}
