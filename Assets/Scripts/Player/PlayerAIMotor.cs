using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RichAI))]
public class PlayerAIMotor : MonoBehaviour
{
    //IAstarAI ai;
    [HideInInspector] public RichAI ai;
    Transform target;
    private float stopDistance = 0f;
    void Start()
    {
        ai = GetComponent<RichAI>();
    }

    public void MoveToPoint(Vector3 point)
    {
      ai.destination = point;
    }

    public void FollowTarget(Interactable interactable)
    {
        target = interactable.interactionPoint;
        stopDistance = interactable.objectRadius;
    }

    public void StopFollowingTarget()
    {
        target = null;
        if (ai.isStopped)
        {
            ai.isStopped = false;
        }
        
    }

    private void Update()
    {
        if(target != null)
        {
            ai.destination = target.position;

            if (Mathf.Abs(Vector3.Distance(ai.position, ai.destination)) < stopDistance)
            {
                ai.isStopped = true;
            }
            else
            {
                ai.isStopped = false;
            }
        }
    }

}
