﻿using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RichAI))]
public class InteractableBrewer : Interactable
{
    private RichAI interactableAI; //Cached AI Component
    private PlayerManager playerManager;
    private ConversationController conversationController;

    private Vector3 agentNextDestination; // Stored next position, to be followed after interaction ends.

    protected override void Start()
    {
        base.Start();
        interactableAI = GetComponent<RichAI>();
        playerManager = PlayerManager.Instance;
        conversationController = GetComponent<ConversationController>();
    }

    public override void Interact()
    {
        agentNextDestination = interactableAI.destination;
        playerManager.onInteractablePlayerFocusedCallback?.Invoke(this.transform);
        interactableAI.enabled = false;
        StartCoroutine(SmoothLookAt(target));
        base.Interact();
        conversationController.ConversationBegan();
    }

    public override void OnDeFocus()
    {
        playerManager.onInteractablePlayerUnFocusedCallback?.Invoke();
        conversationController.ConversationEnded();
        base.OnDeFocus();
        interactableAI.enabled = true;
        interactableAI.canMove = true;
        interactableAI.destination = agentNextDestination;
        interactableAI.SearchPath();
    }

    private IEnumerator SmoothLookAt(Transform targetTransform)
    {
        float inTime = 0.33f;

        Vector3 lookDirection = targetTransform.position - this.transform.position;

        Quaternion toRotation = Quaternion.LookRotation(lookDirection);
        Quaternion fromRotation = this.transform.rotation;

        for (float t = 0; t < inTime; t += Time.deltaTime)
        {
            this.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t / inTime);
            yield return null;
        }

    }
}