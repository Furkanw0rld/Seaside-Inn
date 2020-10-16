using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using System.Collections;
using UnityEngine;

[Category("_AStar")]
[Description("Make the AI interact with in an interactable object, over a time period.")]

public class InteractWithObject : ActionTask<RichAI>
{
    public BBParameter<Interactable> interactableObject;
    public BBParameter<float> minimumTime = 10f;
    public BBParameter<float> maximumTime = 60f;

    private float interactionTime = 0f;

    protected override string info
    {
        get { return interactableObject.value != null ? "Interacting with: " + interactableObject.value.name + "\nInteraction Time: " + interactionTime : "Interactable not set"; }
    }

    protected override void OnExecute()
    {
        agent.canMove = true;
        agent.destination = interactableObject.value.interactionPoint.position;
        interactableObject.value.OnFocus(agent.transform);
        StartCoroutine(InteractWith());
    }

    IEnumerator InteractWith()
    {
        interactionTime = Random.Range(minimumTime.value, maximumTime.value);
        yield return new WaitForSeconds(interactionTime);
        interactableObject.value.OnDeFocus();
        EndAction(true);
    }

    protected override void OnStop()
    {
        interactableObject.value.OnDeFocus();
    }
}
