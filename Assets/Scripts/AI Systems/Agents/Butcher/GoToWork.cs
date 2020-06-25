using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Butcher))]
public class GoToWork : GAction
{
    private Butcher butcher;
    public void Start()
    {
        butcher = GetComponent<Butcher>();
    }

    public override bool PrePerform()
    {
        target = butcher.workplace.transform.position;
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
