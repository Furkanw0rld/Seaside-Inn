using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_AStar")]
[Description("Agent will wander around a radius, randomly. Always will stay within the given radius margin.\n" +
    "If Delay is enabled, Agent will wait at a location for a random specified amount of time between minimum and maximum. This option will not do anything if REPEAT and DELAY is disabled.")]
[Name("Wander Around Location")]
public class WanderAroundAPoint : ActionTask<RichAI>
{
    public BBParameter<Seeker> seeker;
    public BBParameter<Transform> positionToWanderAround;
    public BBParameter<float> maxRadius = 5f;
    public bool repeat = false;
    public bool delayOnFirstRun = false;
    public bool delay = false;
    public float minimumDelay = 1f;
    public float maximumDelay = 5f;

    private bool waitingForNextPath = false;

    protected override void OnExecute()
    {
        agent.canMove = true;
        if (delayOnFirstRun)
        {
            StartCoroutine(DelayedRepeat());
        }
        else
        {
            SetPath();
        }
    }

    protected void SetPath()
    {
        agent.destination = GetWalkablePath();
        agent.SearchPath();
    }

    protected override string info
    {
        get { return positionToWanderAround.value != null ? "Wandering Around:\n" + positionToWanderAround.value.name : "Wandering Location not set."; }
    }

    protected override void OnUpdate()
    {
        if (!waitingForNextPath)
        {
            if (!agent.pathPending && (agent.reachedDestination))
            {
                if (repeat)
                {
                    StartCoroutine(DelayedRepeat());
                }
                else
                {
                    EndAction(true);
                }
            }
        }

    }

    private IEnumerator DelayedRepeat()
    {
        waitingForNextPath = true;
        if (delay)
        {
            agent.isStopped = true;
            yield return new WaitForSeconds(Random.Range(minimumDelay, maximumDelay));
            agent.isStopped = false;
            SetPath();
            waitingForNextPath = false;
        }
        else
        {
            yield return null;
            SetPath();
            waitingForNextPath = false;
        }
    }

    private Vector3 GetWalkablePath()
    {
        GraphNode current = AstarPath.active.data.recastGraph.GetNearest(agent.position, NNConstraint.Default).node;
        GraphNode next = RandomPoint();

        while (!PathUtilities.IsPathPossible(current, next))
        {
            next = RandomPoint();
        }

        return next.RandomPointOnSurface();
    }

    private GraphNode RandomPoint()
    {
        var point = Random.insideUnitSphere * maxRadius.value;
        point.y = 0;
        point += positionToWanderAround.value.position;
        //Check to see if the random position is within the graph
        GraphNode node = AstarPath.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        while (node == null)
        {
            point = Random.insideUnitSphere * maxRadius.value;
            point.y = 0;
            point += positionToWanderAround.value.position;
            node = AstarData.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        }
        return node;
    }

    private Vector3 RandomPointInTourus(Vector3 center, float min, float max)
    {
        Vector2 origin = new Vector2(center.x, center.z);
        Vector2 direction = (Random.insideUnitCircle * origin).normalized; 
        float distance = Random.Range(min, max); 
        Vector2 point = origin + direction * distance; 

        return new Vector3(point.x, center.y, point.y);
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
