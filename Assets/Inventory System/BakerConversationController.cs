using System.Collections;
using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(BakerInventory))]
public class BakerConversationController : ConversationController
{
    private readonly string bakerGreetingsLocation = "Characters/Baker/Dialogues/Greetings_";
    private readonly int amountOfGreetings = 4; //Starts at 0
    private BakerInventory bakerInventory;

    private void Start()
    {
        bakerInventory = GetComponent<BakerInventory>();
    }

    public override IEnumerator ConversationBegan()
    {
        answerOptionA.onClick.AddListener(bakerInventory.OpenShop);
        answerOptionA.onClick.AddListener(CloseShopConversation);
        answerOptionAText.text = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/OpenShop");
        answerOptionB.gameObject.SetActive(false);
        answerOptionC.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f); //Delay from camera transition

        conversationPanel.SetActive(true);
        characterDialogue.text = GetGreetingsMessage();
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
        ClearAllListeners();
        answerOptionAText.text = LocalizationManager.GetTranslation("Characters/Baker/Dialogues/CloseShop");
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
