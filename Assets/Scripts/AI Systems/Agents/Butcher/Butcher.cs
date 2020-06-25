using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher : GAgent
{
    [Header("AI Information")]
    public GameObject workplace;
    public GameObject home;
    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("wanderAround", 1, false);
        goals.Add(s1, 3);
    }

    protected override void Update()
    {
        base.Update();
    }
}
