using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RichAI))]
public class InteractableBaker : Interactable
{
    RichAI interactableAI;
    new private void Start()
    {
        interactableAI = GetComponent<RichAI>();
    }

    public override void Interact()
    {
        interactableAI.enabled = false;
        StartCoroutine(SmoothLookAt(target));
        base.Interact();
    }

    public override void OnDeFocus()
    {
        base.OnDeFocus();
        interactableAI.enabled = true;
    }

    private IEnumerator SmoothLookAt(Transform targetTransform)
    {
        float inTime = 0.33f;
        //Vector3 lookDirection = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        Vector3 lookDirection = targetTransform.position - transform.position;

        //Quaternion toRotation = Quaternion.FromToRotation(transform.forward, lookDirection);
        Quaternion toRotation = Quaternion.LookRotation(lookDirection);
        Quaternion fromRotation = transform.rotation;

        for(float t=0; t < inTime; t+= Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t / inTime);
            yield return null;
        }
    }
}
