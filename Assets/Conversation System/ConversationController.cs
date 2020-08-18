using System.Collections;
using UnityEngine;

public abstract class ConversationController : MonoBehaviour
{
    protected ConversationUI conversationUI;

    public abstract void ConversationBegan();
    public abstract void ConversationEnded();
}
