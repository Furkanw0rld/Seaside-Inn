using Pathfinding;
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
                if (focus)
                {
                    RemoveFocus();
                }

                motor.MoveToPoint(hit.point);
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame) //Interactable 
        {
            ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray, out hit, clickDistance, movementLayerMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if(interactable != null) //Object is interactable
                {
                    SetFocus(interactable);
                }
            }
        }

        MovementTick();
    }

    private Vector2 input;
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.3f;
    float currentSpeed;
    private float speedSmoothVelocity;
    public float speedSmoothTime = 0.15f;
    private void MovementTick()
    {
        Vector2 inputDirection = input.normalized;

        if (inputDirection != Vector2.zero)
        {

            //float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

            //float targetSpeed = motor.ai.maxSpeed * inputDirection.magnitude;
            //currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            //Vector3 nextPosition = transform.position + transform.TransformDirection(transform.forward * currentSpeed * Time.deltaTime);
            //GraphNode node = AstarData.active.data.recastGraph.PointOnNavmesh(nextPosition, NNConstraint.Default);
            //if (node != null)
            //{
            //    transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

            //    if (focus)
            //    {
            //        RemoveFocus();
            //    }
            //}
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //TODO: Rotate Camera
        Debug.Log("Rotating Camera");
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
