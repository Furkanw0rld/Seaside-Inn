using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CMPlayerMotor : MonoBehaviour
{
    public CinemachineFreeLook _freeLookCamera;
    public CinemachineVirtualCamera _zoomedInCamera;
    private Vector2 mouseDelta;

    private void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        PlayerManager.Instance.onInteractablePlayerFocusedCallback += ZoomedInView;
        PlayerManager.Instance.onInteractablePlayerUnFocusedCallback += CancelZoomedInView;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.onInteractablePlayerFocusedCallback -= ZoomedInView;
        PlayerManager.Instance.onInteractablePlayerUnFocusedCallback -= CancelZoomedInView;
    }

    public void ZoomedInView(Transform lookTarget) // When we interact with player, switch cam
    {
        _zoomedInCamera.LookAt = lookTarget;
        _zoomedInCamera.Priority = 15;
    }



    public void CancelZoomedInView() //Revert back to freelook camera
    {
        _zoomedInCamera.Priority = 5;
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
            _freeLookCamera.m_XAxis.m_InputAxisValue = mouseDelta.x;
            _freeLookCamera.m_YAxis.m_InputAxisValue = mouseDelta.y;

            //_freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            //_freeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            _freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
