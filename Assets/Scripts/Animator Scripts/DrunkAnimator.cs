using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkAnimator : MonoBehaviour
{
    Animator animator;
    RichAI agent;
    float speed;
    float idleIndex = 0.0f;
    bool idleFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<RichAI>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = agent.velocity.magnitude / agent.maxSpeed;
        animator.SetFloat("speed", speed);
        if(speed < 0.4f && !idleFlag )
        {
            ChangeIdleIndex();
        }
        
        if(speed > 0.75f && idleFlag)
        {
            idleFlag = false;
        }
    }

    private void ChangeIdleIndex()
    {
        idleIndex = Random.Range(0.0f, 3.0f);
        animator.SetFloat("idleIndex", idleIndex);
        idleFlag = true;
    }
}
