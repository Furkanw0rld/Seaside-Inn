using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HoverOverAI : MonoBehaviour
{
    public TextMeshPro actionText;
    public TextMeshPro characterNameText;

    private GAgent agent;

    private void Start()
    {
        actionText.enabled = false;
        characterNameText.enabled = false;
        agent = this.GetComponent<GAgent>();
    }
    public void OnMouseEnter()
    {
        actionText.enabled = true;
        characterNameText.enabled = true;
        if(agent.currentAction != null)
        {
            actionText.text = agent.currentAction.actionName;
        }
        else
        {
            actionText.text = "Idle";
        }
        
    }

    public void OnMouseOver()
    {
        actionText.transform.rotation = Quaternion.LookRotation(actionText.transform.position - Camera.main.transform.position);
        characterNameText.transform.rotation = Quaternion.LookRotation(characterNameText.transform.position - Camera.main.transform.position);
    }

    public void OnMouseExit()
    {
        actionText.enabled = false;
        characterNameText.enabled = false;
    }

}
