﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : GAgent
{

    new void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("wanderAround", 1, false);
        goals.Add(s1, 3);
    }

}
