using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CMPlayerMotor : MonoBehaviour
{
    private CinemachineFreeLook _freeLook;
    private Vector2 mouseDelta;

    private void Start()
    {
        _freeLook = GetComponent<CinemachineFreeLook>();
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
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
            _freeLook.m_XAxis.m_InputAxisValue = mouseDelta.x;
            _freeLook.m_YAxis.m_InputAxisValue = mouseDelta.y;

            //_freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            //_freeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _freeLook.m_XAxis.m_InputAxisValue = 0;
            _freeLook.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
