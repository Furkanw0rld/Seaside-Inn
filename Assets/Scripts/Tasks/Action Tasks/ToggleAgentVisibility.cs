using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using ParadoxNotion.Design;
using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_Agent")]
[Description("If Hide Agent is set to true, agent will be invisible. If set to false, agent will become visible.")]
[Name("Change Agents Visibility")]
public class ToggleAgentVisibility : ActionTask<Transform>
{
    public bool hideAgent = false;

    protected override void OnExecute()
    {
        StartCoroutine(ChangeVisiblity());
    }

    protected IEnumerator ChangeVisiblity()
    {
        if (hideAgent)
        {
            // Make agent hidden
            agent.GetComponent<CapsuleCollider>().isTrigger = true;
            agent.GetComponent<RichAI>().enabled = false;
            yield return null;

            if(agent.TryGetComponent(out Interactable interactable))
            {
                interactable.enabled = false;
            }
            agent.GetComponent<HoverOverAI>().enabled = false;
            yield return null;

            agent.GetComponent<RVOController>().enabled = false;
            agent.GetComponent<ISkinnedMeshReference>().MeshRenderer.enabled = false;
            yield return null;
        }
        else
        {
            // Make agent visible
            agent.GetComponent<CapsuleCollider>().isTrigger = false;
            agent.GetComponent<RichAI>().enabled = true;
            yield return null;

            if (agent.TryGetComponent(out Interactable interactable))
            {
                interactable.enabled = true;
            }
            agent.GetComponent<HoverOverAI>().enabled = true;
            yield return null;

            agent.GetComponent<RVOController>().enabled = true;
            agent.GetComponent<ISkinnedMeshReference>().MeshRenderer.enabled = true;
            yield return null;
        }

        blackboard.parent.SetVariableValue("isVisible", !hideAgent);
        EndAction(true);
    }

    protected override string info
    {
        get { return hideAgent? "Agent will be hidden" : "Agent will be visible"; }
    }
}
