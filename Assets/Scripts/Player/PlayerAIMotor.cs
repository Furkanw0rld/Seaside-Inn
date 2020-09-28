using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RichAI))] [RequireComponent(typeof(Seeker))]
public class PlayerAIMotor : MonoBehaviour
{
    //Cached AI Components
    [HideInInspector] public RichAI ai;
    [HideInInspector] public Seeker seeker;

    private Transform target; // Transform of the target being followed
    private float stopDistance = 0f; // Distance to stop from the target ( Value is set by the interaction radius set on the interactable object)
    private Vector3 lastPathPosition; // Stores the last node position in the path followed by motor
    void Start()
    {
        ai = GetComponent<RichAI>();
        seeker = GetComponent<Seeker>();
    }

    public void MoveToPoint(Vector3 point)
    {
        if (!ai.enableRotation)
        {
            ai.enableRotation = true;
        }

        ai.canSearch = true;
        seeker.StartPath(transform.position, point);
        ai.SearchPath();
    }

    public void FollowTarget(Interactable interactable)
    {
        if (!ai.enableRotation)
        {
            ai.enableRotation = true;
        }

        target = interactable.interactionPoint;
        stopDistance = interactable.objectRadius;
        ai.canSearch = true;
        seeker.StartPath(transform.position, target.position);
        lastPathPosition = target.position;
    }

    public void StopFollowingTarget()
    {
        target = null;
        ai.SetPath(null);
        ai.canSearch = false;
    }

    private void Update()
    {
        if(target != null)
        {
            if (lastPathPosition.sqrMagnitude != target.position.sqrMagnitude)
            {
                seeker.StartPath(transform.position, target.position);
                lastPathPosition = target.position;
            }

            if (ai.remainingDistance < stopDistance)
            {
                ai.SetPath(null);
                ai.canSearch = false;
            }
        }
    }
}
