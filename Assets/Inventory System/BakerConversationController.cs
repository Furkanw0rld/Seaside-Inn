using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BakerConversationController : ConversationController
{
    private Interactable _interactable;
    protected string[] boughtBreadDialogues = new string[]
    {
        "One fresh bread coming right up.",
        "Sure, that'll be one coin.",
        "Here is your bread.",
        "If you've got the coin, I've got the bread.",
        "Fresh bread, straight out of the stone oven!"
    };

    protected string[] notEnoughCoinDialogues = new string[]
    {
        "You've got no coin to buy bread, dear.",
        "You need coin to buy bread.",
        "Come back and find me when you've got coin.",
        "I do not give bread for free.",
        "Bread isn't free you know?"
    };


    public Item breadItem;

    protected override void Start()
    {
        this.gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        answerOptionA.onClick.AddListener(BuyBreadItem);
        answerOptionB.gameObject.SetActive(false);
        answerOptionC.gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        answerOptionA.onClick.RemoveAllListeners();
        answerOptionB.onClick.RemoveAllListeners();
        answerOptionC.onClick.RemoveAllListeners();
    }

    public void BuyBreadItem()
    {
        if (PlayerInventory.Instance.BuyItem(breadItem, breadItem.itemPrice, 1))
        {
            characterDialogue.text = boughtBreadDialogues[GetRandomDialogue(boughtBreadDialogues.Length)];
            answerOptionB.gameObject.SetActive(true);
            answerOptionB.GetComponent<TextMeshProUGUI>().text = "- Thank you. (Exit Conversation)";
            answerOptionB.onClick.AddListener(PlayerManager.Instance.playerController.RemoveFocus);
        }
        else
        {
            characterDialogue.text = notEnoughCoinDialogues[GetRandomDialogue(notEnoughCoinDialogues.Length)];
        }
    }

    private int GetRandomDialogue(int count)
    {
        return Random.Range(0, count);
    }

    public override IEnumerator ConversationBegan(Interactable interactable)
    {
        _interactable = interactable;
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(true);
        characterDialogue.text = "Hello, there! What can I do for you?";
        //To be overwritten by inherited children
        Debug.Log("Conversation Began. Enumerator.");
    }

    public override void ConversationEnded()
    {
        _interactable = null;
        this.gameObject.SetActive(false);
        Debug.Log("Conversation Ended.");
    }
}
