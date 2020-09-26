using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CMPlayerMotor : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public CinemachineVirtualCamera zoomedInCamera;
    public CinemachineVirtualCamera kitchenCamera;
    private Vector2 mouseDelta;
    private PlayerManager playerManager;
    private InventoryZonesHandler inventoryZones;

    private float rotationTimer = 0f;
    private readonly float minimumTimeToHoldToRotate = 0.3f;

    // Camera Priorities:
    // Free Look Cam is set to 10. (Default Cam)
    private const int STANDBY_PRIORITY = 5;
    private const int KITCHEN_PRIORITY = 15;
    private const int ZOOMED_IN_PRIORITY = 16;

    private void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        playerManager = PlayerManager.Instance;
        inventoryZones = InventoryZonesHandler.Instance;

        playerManager.onInteractablePlayerFocusedCallback += ZoomedInView;
        playerManager.onInteractablePlayerUnFocusedCallback += ExitZoomedInView;

        inventoryZones.onPlayerEnterKitchenAreaCallback += KitchenCameraView;
        inventoryZones.onPlayerExitKitchenAreaCallback += ExitKitchenCameraView;
    }

    private void OnDisable()
    {
        playerManager.onInteractablePlayerFocusedCallback -= ZoomedInView;
        playerManager.onInteractablePlayerUnFocusedCallback -= ExitZoomedInView;

        inventoryZones.onPlayerEnterKitchenAreaCallback -= KitchenCameraView;
        inventoryZones.onPlayerExitKitchenAreaCallback -= ExitKitchenCameraView;
    }

    private void KitchenCameraView()
    {
        kitchenCamera.Priority = KITCHEN_PRIORITY;
    }

    private void ExitKitchenCameraView()
    {
        kitchenCamera.Priority = STANDBY_PRIORITY;
    }

    private void ZoomedInView(Transform lookTarget) // When we interact with player, switch cam
    {
        zoomedInCamera.LookAt = lookTarget;
        zoomedInCamera.Follow = lookTarget;
        zoomedInCamera.Priority = ZOOMED_IN_PRIORITY;
    }

    private void ExitZoomedInView() //Revert back to freelook camera
    {
        zoomedInCamera.Priority = STANDBY_PRIORITY;
    }

    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject()) //Don't rotate if over UI
        {
            return;
        }

        if (Mouse.current.rightButton.isPressed) 
        {
            rotationTimer += Time.deltaTime;

            if(rotationTimer > minimumTimeToHoldToRotate)
            {
                if (Cursor.visible)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                mouseDelta = Mouse.current.delta.ReadValue();
                mouseDelta *= 0.5f; //Account for scaling applied
                mouseDelta *= 0.1f; //Account for sensitivity
                freeLookCamera.m_XAxis.m_InputAxisValue = mouseDelta.x;
                freeLookCamera.m_YAxis.m_InputAxisValue = mouseDelta.y;
            }

        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            rotationTimer = 0f;

            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
            }

        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if(inputValue != Vector2.zero)
        {
            inputValue *= 0.5f;
            inputValue *= 0.1f;
            freeLookCamera.m_XAxis.m_InputAxisValue = inputValue.x;
            freeLookCamera.m_YAxis.m_InputAxisValue = inputValue.y;
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
        }

    }
}
