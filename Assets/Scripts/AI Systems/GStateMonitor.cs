using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GStateMonitor : MonoBehaviour
{
    public string state;
    public float stateStrength;
    public float stateDecayRate;
    public WorldStates beliefs;
    public GameObject resourcePrefab;
    public string queueName;
    public string worldState;
    public GAction action;

    bool stateFound = false;
    float initialStrength;


    private void Awake()
    {
        beliefs = this.GetComponent<GAgent>().beliefs;
        initialStrength = stateStrength;
    }

    private void LateUpdate()
    {
        if (action.running)
        {
            stateFound = false;
            stateStrength = initialStrength;
        }

        if(!stateFound && beliefs.HasState(state))
        {
            stateFound = true;
        }

        if (stateFound)
        {
            stateStrength -= stateDecayRate * Time.deltaTime;
            if(stateStrength <= 0)
            {
                //Instantiate resource here based on state strength

                //then
                stateFound = false;
                stateStrength = initialStrength;
                beliefs.RemoveState(state);
                //add back to queue
                //GWorld.Instance.GetQueue(queueName).AddResource(object);
                //GWorld.Instance.GetWorld().ModifyState(worldState, 1); 
            }
        }
    }
}
