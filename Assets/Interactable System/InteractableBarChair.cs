using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBarChair : Interactable
{
    public Transform exitPoint = null;
    public bool IsOccupied { get; private set; }

    public override void Interact()
    {
        IsOccupied = true;
        base.Interact();
        Sitting();
    }

    private void Sitting()
    {
        targetAI.enabled = false;
        playerAnimator.Sitting(true);
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
            IsOccupied = false;
        }

        if (isFocused)
        {
            targetAI.enabled = true;
            playerAnimator.Sitting(false);
        }

        base.OnDeFocus();
    }
}
