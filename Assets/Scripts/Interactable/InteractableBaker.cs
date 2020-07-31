using Pathfinding;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RichAI), typeof(HoverOverAI))]
public class InteractableBaker : Interactable
{
    private RichAI interactableAI; //Cached AI Component
    private HoverOverAI hoverOverAI;
#pragma warning disable 0649 
    [SerializeField] private ConversationController conversationController;
#pragma warning restore 0649 
    new private void Start()
    {
        interactableAI = GetComponent<RichAI>();
        hoverOverAI = GetComponent<HoverOverAI>();
    }

    public override void Interact()
    {
        PlayerManager.Instance.onInteractablePlayerFocusedCallback?.Invoke(this.transform);
        interactableAI.enabled = false;
        StartCoroutine(SmoothLookAt(target));
        base.Interact();
        StartCoroutine(conversationController.ConversationBegan());
    }

    public override void OnDeFocus()
    {
        PlayerManager.Instance.onInteractablePlayerUnFocusedCallback?.Invoke();
        conversationController.ConversationEnded();
        base.OnDeFocus();
        interactableAI.enabled = true;
    }

    private IEnumerator SmoothLookAt(Transform targetTransform)
    {
        float inTime = 0.33f;

        Vector3 lookDirection = targetTransform.position - this.transform.position;

        Quaternion toRotation = Quaternion.LookRotation(lookDirection);
        Quaternion fromRotation = this.transform.rotation;

        for(float t=0; t < inTime; t+= Time.deltaTime)
        {
            this.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t / inTime);
            yield return null;
        }

    }
}
