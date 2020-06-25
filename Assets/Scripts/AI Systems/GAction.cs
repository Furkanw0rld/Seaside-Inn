using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";
    public float cost = 1f;
    [HideInInspector] public Vector3 target; //the target position
    public float duration = 0;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    [HideInInspector] public RichAI agent;

    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;

    public WorldStates beliefs;

    public GInventory inventory;

    public bool running = false;

    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = this.gameObject.GetComponent<RichAI>();

        if (preConditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }
        }

        inventory = this.GetComponent<GAgent>().inventory;
        beliefs = this.GetComponent<GAgent>().beliefs;
    }

    public bool IsAchievable() //Can reject actions here based on some conditions
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions) 
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            if (!conditions.ContainsKey(p.Key))
            {
                return false; //Return false if there are no matching conditions
            }
        }

        return true; //Otherwise return true
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}
