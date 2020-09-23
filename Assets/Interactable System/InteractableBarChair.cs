using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBarChair : Interactable
{
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
        target.rotation = Quaternion.LookRotation(-interactionPoint.up);

    }

    public override void OnDeFocus()
    {
        if (isFocused)
        {
            targetAI.enabled = true;
            playerAnimator.Sitting(false);
        }

        if (hasInteracted)
        {
            target.position = this.interactionPoint.parent.position;
            IsOccupied = false;
        }

        base.OnDeFocus();
    }
}
