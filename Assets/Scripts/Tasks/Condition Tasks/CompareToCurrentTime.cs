using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_GameTimeSystem")]
[Description("Compare a time against the current day time. (0f - 1440f). Returns true if current time is less than the time specified.")]
public class CompareToCurrentTime : ConditionTask
{
    public BBParameter<float> time;
    private GameTimeManager gameTime;

    protected override string info
    {
        get { return "Current time less than: " + time; }
    }

    protected override string OnInit()
    {
        gameTime = GameTimeManager.Instance;
        return null;
    }

    protected override bool OnCheck()
    {
        return time.value >= gameTime.GetCurrentTime();
    }
}
