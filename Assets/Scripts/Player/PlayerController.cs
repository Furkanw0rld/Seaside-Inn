﻿using Pathfinding;
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
    private RecastGraph activeRecastGraphData; // Cached Player Movement Graph
    private Vector2 movementInput; // Stores the players current keyboard/joystick input per frame
    private Vector3 finalizedMovement; //Stores the final movement-position delta per frame.
    private Vector3 camForward; // Camera Forward Direction per frame
    private Vector3 camRelativeMovement; // Camera Relative Movement per frame
    private Vector3 nextPosition; // Stores the next position per frame
    private GraphNode positionNode; // Stores the next node to check against

    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerAIMotor>();
        playerMaximumSpeed = motor.ai.maxSpeed;
        activeRecastGraphData = AstarData.active.data.recastGraph;
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
    // TODO: Add Acceleration to player movement override. 
    private void MovementTick()
    {
        if (movementInput != Vector2.zero)
        {
            if (movementInput.x >= 0.1f || movementInput.x <= -0.1f || movementInput.y >= 0.1f || movementInput.y <= -0.1f) //Dead-zones
            {
                camForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized; //Camera-Forward Direction
                camRelativeMovement = (movementInput.x * cam.transform.right) + (movementInput.y * camForward); //Camera relative movement direction
                finalizedMovement = camRelativeMovement * Time.deltaTime * playerMaximumSpeed; //Finalized movement position delta

                nextPosition = transform.position + finalizedMovement;

                positionNode = activeRecastGraphData.PointOnNavmesh(nextPosition, NNConstraint.Default);

                if (positionNode != null)
                {
                    // Clear path, and override movement controls to player
                    motor.ai.canSearch = false;
                    motor.ai.SetPath(null);
                    motor.ai.Move(finalizedMovement);

                    if (camRelativeMovement != Vector3.zero)
                    {
                        transform.forward = camRelativeMovement;
                    }

                    if (focus)
                    {
                        RemoveFocus();
                    }
                }
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
