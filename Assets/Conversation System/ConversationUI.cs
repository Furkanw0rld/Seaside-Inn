using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConversationUI : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject conversationUIPanel;
    [Header("Dialogue Options")]
    [SerializeField] [Tooltip("Characters Response.")] private TextMeshProUGUI characterDialogue;
    [Space]
    [SerializeField] [Tooltip("Top choice.")] private Button answerOptionA;
    [SerializeField] [Tooltip("Text component of answer option A.")] private TextMeshProUGUI answerOptionAText;
    [Space]
    [SerializeField] [Tooltip("Middle choice.")] private Button answerOptionB;
    [SerializeField] [Tooltip("Text component of answer option B.")] private TextMeshProUGUI answerOptionBText;
    [Space]
    [SerializeField] [Tooltip("Bottom choice.")] private Button answerOptionC;
    [SerializeField] [Tooltip("Text component of answer option C. ")] private TextMeshProUGUI answerOptionCText;
#pragma warning restore 0649

    public static ConversationUI Instance;

    private readonly float cameraTransitionDelay = 2f; // Time between the transition of cameras

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (conversationUIPanel.activeSelf)
        {
            conversationUIPanel.SetActive(false);
        }
    }

    public IEnumerator ShowConversationUI(Interactable interactable, 
                            string characterDialogueText,
                            string answerAText = null, 
                            UnityAction[] answerACallbacks = null,
                            string answerBText = null, 
                            UnityAction[] answerBCallbacks = null,
                            string answerCText = null, 
                            UnityAction[] answerCCallbacks = null)
    {
        ClearAllListeners();

        characterDialogue.text = characterDialogueText;

        if(answerACallbacks != null)
        {
            answerOptionAText.text = answerAText;
            answerOptionA.gameObject.SetActive(true);

            foreach(UnityAction action in answerACallbacks)
            {
                answerOptionA.onClick.AddListener(action);
            }

        }
        else
        {
            answerOptionA.gameObject.SetActive(false);
        }

        if(answerBCallbacks != null)
        {
            answerOptionBText.text = answerBText;
            answerOptionB.gameObject.SetActive(true);

            foreach(UnityAction action in answerBCallbacks)
            {
                answerOptionB.onClick.AddListener(action);
            }
        }
        else
        {
            answerOptionB.gameObject.SetActive(false);
        }

        if(answerCCallbacks != null)
        {
            answerOptionCText.text = answerCText;
            answerOptionC.gameObject.SetActive(true);
            
            foreach(UnityAction action in answerCCallbacks)
            {
                answerOptionC.onClick.AddListener(action);
            }
        }
        else
        {
            answerOptionC.gameObject.SetActive(false);
        }


        yield return new WaitForSeconds(cameraTransitionDelay);

        if (interactable.IsInteracting())
        {
            conversationUIPanel.SetActive(true);
        }
        else
        {
            conversationUIPanel.SetActive(false);
        }
    }

    public void CloseConversationUI()
    {
        conversationUIPanel.SetActive(false);
    }

    private void ClearAllListeners()
    {
        answerOptionA.onClick.RemoveAllListeners();
        answerOptionB.onClick.RemoveAllListeners();
        answerOptionC.onClick.RemoveAllListeners();
    }
}
