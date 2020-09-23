using Pathfinding;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [Tooltip("Radius of the interactable object.")]public float objectRadius = 1.5f;
    [Tooltip("If left empty, interaction point will be where this script is attached to.")] public Transform interactionPoint;

    protected bool isFocused = false;
    // Targets--
    protected Transform target;
    protected PlayerAnimator playerAnimator;
    protected RichAI targetAI;

    protected bool hasInteracted = false;

    protected virtual void Start()
    {
        if(interactionPoint == null)
        {
            interactionPoint = this.transform;
        }
    }

    public virtual void Interact()
    {
        // TODO: Make Interact Abstract
        Debug.Log(target.name + " interacting with " + transform.name);
        //Overwritten
    }

    public bool IsInteracting()
    {
        return (hasInteracted && isFocused);
    }

    private void Update()
    {
        if (isFocused && !hasInteracted)
        {
            float distance = Vector3.Distance(target.position, interactionPoint.position);
            if(distance <= objectRadius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void OnFocus(Transform playerTransform)
    {
        isFocused = true;
        target = playerTransform;
        targetAI = playerTransform.GetComponent<RichAI>();
        playerAnimator = playerTransform.GetComponent<PlayerAnimator>();
        hasInteracted = false;
    }

    public virtual void OnDeFocus()
    {
        isFocused = false;
        target = null;
        targetAI = null;
        playerAnimator = null;
        hasInteracted = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(interactionPoint.position, objectRadius);
    }
}
