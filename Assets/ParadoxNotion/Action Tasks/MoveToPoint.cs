using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_AStar/Movement")]
public class MoveToPoint : ActionTask<RichAI>
{
    public BBParameter<Vector3> target;

    protected override string info
    {
        get { return "Move To Position: \n" + target.value; }
    }

    protected override void OnExecute()
    {
        if(target.value == null)
        {
            EndAction(false);
        }
        else
        {
            agent.canMove = true;
            agent.destination = target.value;
            agent.SearchPath();
        }
    }

    protected override void OnUpdate()
    {
        if(!agent.pathPending && (agent.reachedEndOfPath || !agent.hasPath))
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
