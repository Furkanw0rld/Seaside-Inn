using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_GameTimeSystem")]
[Description("Will cause the agent to wait until the certain time is reached.\n Will check every X seconds to see if the time is met.")]
public class WaitUntilCurrentTime : ActionTask
{
    public BBParameter<float> timeToWaitUntil;
    public BBParameter<float> timeBetweenChecks = 5f;

    private GameTimeManager gameTime;

    protected override string OnInit()
    {
        gameTime = blackboard.GetVariable<GameTimeManager>("gameTimeManager").value;
        return null;
    }

    protected override string info
    {
        get { return "Waiting until: " + timeToWaitUntil; }
    }

    protected override void OnExecute()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        while(timeToWaitUntil.value >= gameTime.GetCurrentTime())
        {
            yield return new WaitForSeconds(timeBetweenChecks.value);
        }

        EndAction(true);
    }
}
