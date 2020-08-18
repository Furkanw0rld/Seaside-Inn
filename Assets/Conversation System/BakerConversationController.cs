using System.Collections;
using UnityEngine;
using I2.Loc;
using UnityEngine.Events;

[RequireComponent(typeof(BakerInventory), typeof(InteractableBaker))]
public class BakerConversationController : ConversationController
{
    private readonly string bakerGreetingsLocation = "Characters/Baker/Dialogues/Greetings_";
    private readonly int amountOfGreetings = 4; //Starts at 0
    private BakerInventory bakerInventory;
    private InteractableBaker interactableBaker;

    private void Start()
    {
        bakerInventory = GetComponent<BakerInventory>();
        interactableBaker = GetComponent<InteractableBaker>();

        conversationUI = ConversationUI.Instance;
    }

    public override void ConversationBegan()
    {
        string dialogue = GetGreetingsMessage();
        string openShopDialogue = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/OpenShop");
        UnityAction[] answerACallbacks = new UnityAction[] { bakerInventory.OpenShop, CloseShopConversation };
        StartCoroutine(conversationUI.ShowConversationUI(interactableBaker, dialogue, openShopDialogue, answerACallbacks));
    }


    private string GetGreetingsMessage()
    {
        int randomChoice = Random.Range(0, amountOfGreetings);
        if(LocalizationManager.TryGetTranslation(bakerGreetingsLocation + randomChoice, out string localization))
        {
            return localization;
        }
        else
        {
            return "Hello!";
        }
    }

    public void CloseShopConversation()
    {
        string optionAText = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/CloseShop");
        UnityAction[] answerACallbacks = new UnityAction[] { interactableBaker.OnDeFocus, PlayerManager.Instance.playerController.RemoveFocus };
        StartCoroutine(conversationUI.ShowConversationUI(interactableBaker, "Buy whatever you'd like.", optionAText, answerACallbacks));
    }

    public override void ConversationEnded()
    {
        conversationUI.CloseConversationUI();
        bakerInventory.CloseShop();
    }
    
}
