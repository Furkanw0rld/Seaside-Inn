using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAround : GAction
{
    public override bool PrePerform()
    {
        target = RandomPoint();
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    private Vector3 RandomPoint()
    {
        var point = Random.insideUnitSphere * 60f;
        point.y = 0;
        point += this.transform.position;
        //Check to see if the random position is within the graph
        GraphNode node = AstarData.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        while (node == null)
        {
            point = Random.insideUnitSphere * 60f;
            point.y = 0;
            point += this.transform.position;
            node = AstarData.active.data.recastGraph.PointOnNavmesh(point, NNConstraint.Default);
        }
        return (Vector3)node.RandomPointOnSurface();
    }
}
