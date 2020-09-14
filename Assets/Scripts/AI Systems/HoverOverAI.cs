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

    public void OnMouseEnter()
    {
        actionText.enabled = true;
        characterNameText.enabled = true;
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
