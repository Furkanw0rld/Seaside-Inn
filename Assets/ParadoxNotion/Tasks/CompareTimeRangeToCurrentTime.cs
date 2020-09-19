using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_GameTimeSystem")]
[Description("Compare a time range against current time.\n Will return true only if current time is between the specified minimum and maximum.")]
public class CompareTimeRangeToCurrentTime : ConditionTask
{
    public BBParameter<float> timeMinimum;
    public BBParameter<float> timeMaximum;
    private GameTimeManager gameTime;

    protected override string info
    {
        get { return "Current time is between\n" + timeMinimum + " and " + timeMaximum; }
    }

    protected override string OnInit()
    {
        gameTime = blackboard.GetVariable<GameTimeManager>("gameTimeManager").value;
        return null;
    }

    protected override bool OnCheck()
    {
        float currentTime = gameTime.GetCurrentTime();
        return (timeMinimum.value <= currentTime && timeMaximum.value >= currentTime);
    }
}
