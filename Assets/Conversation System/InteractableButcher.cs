using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(RichAI), typeof(ConversationControllerButcher))]
public class InteractableButcher : Interactable
{
    private RichAI interactableAI;
    private PlayerManager playerManager;
    private ConversationController conversationController;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        interactableAI = this.GetComponent<RichAI>();
        playerManager = PlayerManager.Instance;
        conversationController = GetComponent<ConversationControllerButcher>();
    }

    public override void Interact()
    {
        base.Interact();
        playerManager.onInteractablePlayerFocusedCallback?.Invoke(this.transform);
        interactableAI.isStopped = true;
        interactableAI.enableRotation = false;
        StartCoroutine(SmoothLookAt(target));
        conversationController.ConversationBegan();
    }

    public override void OnDeFocus()
    {
        playerManager.onInteractablePlayerUnFocusedCallback?.Invoke();
        base.OnDeFocus();
        interactableAI.isStopped = false;
        interactableAI.enableRotation = true;
        conversationController.ConversationEnded();
    }

    private IEnumerator SmoothLookAt(Transform targetTransform)
    {
        float inTime = 1f;

        Vector3 lookDirection = targetTransform.position - this.transform.position;

        Quaternion toRotation = Quaternion.Euler(0, Quaternion.LookRotation(lookDirection).eulerAngles.y, 0); 
        Quaternion fromRotation = this.transform.rotation;

        for (float t = 0; t < inTime; t += Time.deltaTime)
        {
            this.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t / inTime);
            yield return null;
        }

    }
}
