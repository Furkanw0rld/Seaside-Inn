using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour, ISittingAnimator
{
    Animator animator;
    RichAI agent;
    float speed;
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
    }

    public void Sleeping(bool flag)
    {
        animator.SetBool("isLaying", flag);
    }

    public void Sitting(bool flag)
    {
        animator.SetBool("isSitting", flag);
    }
}
