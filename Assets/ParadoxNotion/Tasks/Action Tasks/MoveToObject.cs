using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_AStar/Movement")]
[Description("Move the Agent to a GameObject's Transform.")]
public class MoveToObject : ActionTask<RichAI>
{
    public BBParameter<Transform> target;

    protected override string info
    {
        get { return (target.value != null)? "Moving to Object: \n" + target.value.name + "\nLocation: " + target.value.position : "No Object Assigned"; }
    }

    protected override void OnExecute()
    {
        if(!target.value)
        {
            EndAction(false);
        }
        else
        {
            agent.canMove = true;
            agent.destination = target.value.position;
            agent.SearchPath();
        }
    }

    protected override void OnUpdate()
    {
        if (!agent.pathPending && (agent.reachedEndOfPath || !agent.hasPath))
        {
            EndAction(true);
        }
    }

    protected override void OnPause()
    {
        OnStop();
    }

    protected override void OnStop()
    {
        agent.destination = agent.position;
    }
}
