using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Category("_GameTimeSystem")]
[Description("Compare a time against the World Time, returns true if the time has already past.")]
public class CompareToWorldTime : ConditionTask
{
    public BBParameter<ulong> time;

    private GameTimeManager gameTime;

    protected override string OnInit()
    {
        gameTime = GameTimeManager.Instance;
        return null;
    }

    protected override bool OnCheck()
    {
        return gameTime.CompareTimeToWorld(time.value);
    }

}
