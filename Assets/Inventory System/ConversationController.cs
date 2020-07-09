using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ConversationController : MonoBehaviour
{
    [SerializeField] [Tooltip("Game Object of the UI Elements.")] protected GameObject conversationPanel;
    [SerializeField] [Tooltip("Characters Response.")] protected TextMeshProUGUI characterDialogue;
    [SerializeField] [Tooltip("Top choice.")] protected Button answerOptionA;
    [SerializeField] [Tooltip("Text component of answer option A.")] protected TextMeshProUGUI answerOptionAText;
    [SerializeField] [Tooltip("Middle choice.")] protected Button answerOptionB;
    [SerializeField] [Tooltip("Text component of answer option B.")] protected TextMeshProUGUI answerOptionBText;
    [SerializeField] [Tooltip("Bottom choice.")] protected Button answerOptionC;
    [SerializeField] [Tooltip("Text component of answer option C. ")] protected TextMeshProUGUI answerOptionCText;
    public abstract IEnumerator ConversationBegan();
    public abstract void ConversationEnded();
}
