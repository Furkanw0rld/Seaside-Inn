using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Blackboard))]
public class ButcherAgent : MonoBehaviour
{
    [Header("Agent Information")]
    public ushort productionTime = 720;
    
    private Blackboard blackboard;

    public void Initialize()
    {
        blackboard = GetComponent<Blackboard>();
        blackboard.GetVariable("productionTime").value = productionTime;
    }

}
