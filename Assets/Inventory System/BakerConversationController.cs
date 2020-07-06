using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakerConversationController : ConversationController
{
    protected string[] buyBreadDialogues = new string[5]
    {
        "One fresh bread coming right up.",
        "Sure, that'll be one coin.",
        "Here is your bread.",
        "If you've got the coin, I've got the bread.",
        "Fresh bread, straight out of the stone oven!"
    };

    public Item breadItem; //Item the character sells

    protected override void Start()
    {
        this.gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        answerOptionA.onClick.AddListener(BuyBreadItem);
    }

    protected override void OnDisable()
    {
        answerOptionA.onClick.RemoveAllListeners();
    }

    public void BuyBreadItem()
    {
        if (PlayerInventory.Instance.BuyItem(breadItem, breadItem.itemPrice, 1))
        {
            int randomChoice = Random.Range(0, buyBreadDialogues.Length);
            characterDialogue.text = buyBreadDialogues[randomChoice];
        }
        else
        {
            characterDialogue.text = "You've got no coin to buy bread, dear.";
        }
    }

    public override IEnumerator ConversationBegan()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(true);
        characterDialogue.text = "Hello, there! What can I do for you?";
        //To be overwritten by inherited children
        Debug.Log("Conversation Began. Enumerator.");
    }

    public override void ConversationEnded()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Conversation Ended.");
    }
}
