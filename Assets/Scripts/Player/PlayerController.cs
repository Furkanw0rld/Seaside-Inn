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

    // Player Movement Override (MovementTick) Variables
    private float playerMaximumSpeed; // This value is cached from motor.
    private Vector2 movementInput; // Stores the players current keyboard/joystick input per frame
    private Vector3 finalizedMovement; //Stores the final movement-position delta per frame.
    private Vector3 camForward; // Camera Forward Direction per frame
    private Vector3 camRelativeMovement; // Camera Relative Movement per frame

    private float playerAccelerationRate; // Cached acceleration rate
    private float elapsedAccelerationTime = 0f; // Stores the elapsed time while accelerating
    private float timeForMaximumSpeed; // Stores acceleration time
    private float currentSpeed; // Stores the speed while player accelerates

    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerAIMotor>();

        playerMaximumSpeed = motor.ai.maxSpeed;
        playerAccelerationRate = motor.ai.acceleration;
        timeForMaximumSpeed = playerMaximumSpeed / playerAccelerationRate;
    }

    void Update()
    {
        MovementTick();

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
    }

    // Movement Tick overrides player movement with input via Keyboard/Joystick. However, both options are still accessible to player and can be used interchangably. 
    private void MovementTick()
    {
        if (movementInput != Vector2.zero)
        {
            if (movementInput.x >= 0.1f || movementInput.x <= -0.1f || movementInput.y >= 0.1f || movementInput.y <= -0.1f) //Dead-zones of gamepad
            {

                if (elapsedAccelerationTime < timeForMaximumSpeed)
                {
                    elapsedAccelerationTime += Time.deltaTime;
                    currentSpeed = playerAccelerationRate * elapsedAccelerationTime;
                }
                else
                {
                    currentSpeed = playerMaximumSpeed;
                }

                camForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized; //Camera-Forward Direction
                camRelativeMovement = (movementInput.x * cam.transform.right) + (movementInput.y * camForward); //Camera relative movement direction
                finalizedMovement = camRelativeMovement * Time.deltaTime * currentSpeed; //Finalized movement position delta

                // Clear path, and override movement controls to player
                motor.ai.canSearch = false;
                motor.ai.SetPath(null);
                motor.ai.Move(finalizedMovement);

                if (camRelativeMovement != Vector3.zero)
                {
                    if (motor.ai.enableRotation)
                    {
                        motor.ai.enableRotation = false;
                    }

                    transform.rotation = motor.ai.SimulateRotationTowards(camRelativeMovement, 360f * Time.deltaTime);
                }

                if (focus)
                {
                    RemoveFocus();
                }
            }
        }
        else
        {
            if(elapsedAccelerationTime > 0)
            {
                elapsedAccelerationTime = 0f; // Set time to zero when there is no input.
            }

        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
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
