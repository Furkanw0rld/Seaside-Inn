using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using Pathfinding;
using ParadoxNotion.Design;

[Category("_AStar/Movement")]
public class WanderAround : ActionTask<RichAI>
{
    public BBParameter<float> maximumDistance = 50f;
    public bool repeat = false;
    public BBParameter<Seeker> seeker;


    protected override void OnExecute()
    {
        agent.canMove = true;
        SetPath();
    }

    protected void SetPath()
    {
        agent.destination = GetWalkablePath();
        agent.SearchPath();
    }

    protected override string info
    {
        get { return "Wander Destination: \n" + agent.destination; }
    }

    protected override void OnUpdate()
    {
        if (!agent.pathPending && (agent.reachedDestination))
        {
            if (repeat)
            {
                SetPath();
            }
            else
            {
                EndAction(true);
            }
        }
    }

    private Vector3 GetWalkablePath()
    {
        GraphNode current = AstarPath.active.data.recastGraph.GetNearest(agent.position, NNConstraint.Default).node;
        GraphNode next = RandomPoint();

        while(!PathUtilities.IsPathPossible(current, next))
        {
            next = RandomPoint();
        }

        return next.RandomPointOnSurface();
    }

    private GraphNode RandomPoint()
    {
        var point = Random.insideUnitSphere * maximumDistance.value;
        point.y = 0;
        point += agent.position;
        //Check to see if the random position is within the graph
        GraphNode node = AstarPath.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        while (node == null)
        {
            point = Random.insideUnitSphere * maximumDistance.value;
            point.y = 0;
            point += agent.position;
            node = AstarData.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        }
        return node;
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
