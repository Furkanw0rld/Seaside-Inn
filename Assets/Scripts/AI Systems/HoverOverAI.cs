using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HoverOverAI : MonoBehaviour
{
    public TextMeshPro actionText;
    public TextMeshPro characterNameText;

    private void Start()
    {
        actionText.enabled = false;
        characterNameText.enabled = false;
        actionText.text = "Idle";

    }

    public void SetCurrentAction(string action)
    {
        actionText.text = action;
    }

    private void OnDisable()
    {
        actionText.enabled = false;
        characterNameText.enabled = false;
    }

    public void OnMouseEnter()
    {
        if (!enabled)
        {
            return;
        }

        actionText.enabled = true;
        characterNameText.enabled = true;
    }

    public void OnMouseOver()
    {
        if (!enabled)
        {
            return;
        }

        actionText.transform.rotation = Quaternion.LookRotation(actionText.transform.position - Camera.main.transform.position);
        characterNameText.transform.rotation = Quaternion.LookRotation(characterNameText.transform.position - Camera.main.transform.position);
    }

    public void OnMouseExit()
    {
        actionText.enabled = false;
        characterNameText.enabled = false;
    }

}
