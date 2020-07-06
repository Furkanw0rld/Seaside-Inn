using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ConversationController : MonoBehaviour
{
    [SerializeField][Tooltip("Characters Response.")] protected TextMeshProUGUI characterDialogue;

    [SerializeField][Tooltip("Contains TextMeshProUGUI for text on the same gameObject. Top choice.")] protected Button answerOptionA; 
    [SerializeField][Tooltip("Contains TextMeshProUGUI for text on the same gameObject. Middle choice.")] protected Button answerOptionB;
    [SerializeField][Tooltip("Contains TextMeshProUGUI for text on the same gameObject. Bottom choice.")] protected Button answerOptionC;

    protected abstract void Start();

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    public abstract IEnumerator ConversationBegan();

    public abstract void ConversationEnded();
}
