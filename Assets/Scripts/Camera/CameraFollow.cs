using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
	//Hierarcy:
	// Camera Prefab
	//      Pivot
	//          CAMERA 

	[Range(0f, 10f)] [SerializeField] private float turnSpeed = 2f; //Speed at which Camera will Rotate
	[SerializeField] private float minTilt = 20f; //Minimum Tilt (X-Axis)
	[SerializeField] private float maxTilt = 60f; //Maximum Tilt (X-Axis)


	private GameObject player; //Target to follow

	private float lookAngle;
	private float tiltAngle;
	private Vector3 pivotEulers;
	private Quaternion pivotTargetRot;
	private Quaternion transformTargetRot;

	private Transform mainCamera; //Camera 
	private Transform pivotPoint; //Pivot Position for Camera
	private bool gamepadFlag = false;

	void Awake()
	{
		mainCamera = GetComponentInChildren<Camera>().transform;
		pivotPoint = mainCamera.parent;
		pivotEulers = pivotPoint.rotation.eulerAngles;

		pivotTargetRot = pivotPoint.transform.localRotation;
		transformTargetRot = transform.localRotation;
	}

	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player"); //Set Target
		if(Gamepad.current != null)
		{
			gamepadFlag = true;
		}
		else
		{
			gamepadFlag = false;
		}
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		
		if (player == null)
		{
			return;
		}

		transform.position = player.transform.position; //Move the Camera towards player

	}

	void Update()
	{
        if (EventSystem.current.IsPointerOverGameObject()) //Don't rotate if over UI
        {
			return;
        }

		if (Mouse.current.rightButton.wasPressedThisFrame) //Right Mouse Clicked, Lock Mouse and Rotate Camera
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			RotateCamera();
		}

		if (Mouse.current.rightButton.isPressed || gamepadFlag) //Rotate Camera While Right Mouse is Pressed
		{
			if (Mouse.current.rightButton.isPressed || (Gamepad.current.rightStick.ReadValue().x >= 0.1f || Gamepad.current.rightStick.ReadValue().x <= -0.1f) || (Gamepad.current.rightStick.ReadValue().y >= 0.1f || Gamepad.current.rightStick.ReadValue().y <= -0.1f))
			{
				RotateCamera();
			}
			
		}

		if (Mouse.current.rightButton.wasReleasedThisFrame) //Unlock Mouse when Right Mouse is released
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

	}

	private void RotateCamera()
	{
		//Get mouse position
		Vector2 mouseDelta = Mouse.current.delta.ReadValue();
		if(gamepadFlag && mouseDelta == new Vector2(0,0))
		{
			mouseDelta = Gamepad.current.rightStick.ReadValue();
		}

		lookAngle += mouseDelta.x * turnSpeed;
		//Rotate around Y-Axis Only
		transformTargetRot = Quaternion.Euler(0f, lookAngle, 0f);

		tiltAngle -= mouseDelta.y * turnSpeed; //Create Tilt
		tiltAngle = Mathf.Clamp(tiltAngle, minTilt, maxTilt); //Limit within our range

		pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y, pivotEulers.z);
		pivotPoint.localRotation = pivotTargetRot;
		transform.localRotation = transformTargetRot;

	}
}
