using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAIMotor))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Layers player can walk (hit) upon")] public LayerMask movementLayerMask; //Layers we can walk upon
    [Tooltip("How far in the scene can the player click (in-game meters)")] public float clickDistance = 100f; //How far will the mouse click travel in-game (world units)

    private Camera cam; //Main Camera 
    private Ray ray;
    private RaycastHit hit;

    private PlayerAIMotor motor; //The Player-Movement Driver

    public Interactable focus;
    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerAIMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //If we are interacting with UI
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.leftButton.isPressed) //Move Player 
        {
            ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, out hit, clickDistance, movementLayerMask)) 
            {

                motor.MoveToPoint(hit.point);
                RemoveFocus();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame) //Interactable 
        {
            ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, out hit, clickDistance))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null) //Object is interactable
                {
                    SetFocus(interactable);
                }
            }
        }
    }
    private void SetFocus(Interactable newFocus)
    {
        if(newFocus != focus) //If we are focused on a new interactable object
        {
            if(focus != null) //if our focus isn't null call defocus
            {
                focus.OnDeFocus();
            }
      
            this.focus = newFocus;
            motor.FollowTarget(newFocus);
        }
 
        newFocus.OnFocus(this.transform);

    }

    public void RemoveFocus()
    {
        if(focus != null)
        {
            focus.OnDeFocus();

        }

        focus = null;
        motor.StopFollowingTarget();
    }
}
