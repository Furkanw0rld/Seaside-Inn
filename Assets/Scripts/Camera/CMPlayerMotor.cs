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

    private void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        playerManager = PlayerManager.Instance;

        playerManager.onInteractablePlayerFocusedCallback += ZoomedInView;
        playerManager.onInteractablePlayerUnFocusedCallback += ExitZoomedInView;

        playerManager.onPlayerEnterKitchenAreaCallback += KitchenCameraView;
        playerManager.onPlayerExitKitchenAreaCallback += ExitKitchenCameraView;
    }

    private void OnDisable()
    {
        playerManager.onInteractablePlayerFocusedCallback -= ZoomedInView;
        playerManager.onInteractablePlayerUnFocusedCallback -= ExitZoomedInView;

        playerManager.onPlayerEnterKitchenAreaCallback -= KitchenCameraView;
        playerManager.onPlayerExitKitchenAreaCallback -= ExitKitchenCameraView;
    }

    private void KitchenCameraView()
    {
        kitchenCamera.Priority = 15;
    }

    private void ExitKitchenCameraView()
    {
        kitchenCamera.Priority = 5;
    }

    private void ZoomedInView(Transform lookTarget) // When we interact with player, switch cam
    {
        zoomedInCamera.LookAt = lookTarget;
        zoomedInCamera.Follow = lookTarget;
        zoomedInCamera.Priority = 15;
    }

    private void ExitZoomedInView() //Revert back to freelook camera
    {
        zoomedInCamera.Priority = 5;
    }



    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject()) //Don't rotate if over UI
        {
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Mouse.current.rightButton.isPressed) 
        {
            mouseDelta = Mouse.current.delta.ReadValue();
            mouseDelta *= 0.5f; //Account for Scaling applied
            mouseDelta *= 0.1f; //Account for sensitivity
            freeLookCamera.m_XAxis.m_InputAxisValue = mouseDelta.x;
            freeLookCamera.m_YAxis.m_InputAxisValue = mouseDelta.y;

            //_freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            //_freeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
