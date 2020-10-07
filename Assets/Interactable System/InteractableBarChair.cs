using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBarChair : Interactable
{
    public Transform exitPoint = null;
    public bool IsOccupied { get; private set; }

    private ISittingAnimator sittingAnim;

    public override void Interact()
    {
        IsOccupied = true;
        base.Interact();
        sittingAnim = target.GetComponent<ISittingAnimator>();
        Sitting();
    }

    private void Sitting()
    {
        targetAI.enabled = false;
        sittingAnim.Sitting(true);
        target.position = this.interactionPoint.position;
        target.rotation = Quaternion.LookRotation(transform.forward);

    }

    public override void OnDeFocus()
    {
        if (hasInteracted)
        {

            if (exitPoint)
            {
                target.position = exitPoint.position;
            }
            else
            {
                target.position = (Vector3)AstarData.active.data.recastGraph.GetNearest(this.interactionPoint.position).node.position;
            }

            if (isFocused)
            {
                targetAI.enabled = true;
                sittingAnim.Sitting(false);

            }

            IsOccupied = false;
        }
        base.OnDeFocus();
    }
}
