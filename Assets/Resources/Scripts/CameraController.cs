using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
	public Transform focus;
	public Vector3 offset = Vector3.zero;
	[Range(0f, 1f)] [SerializeField] private float lerpAmount = 0.9f;
	private Transform cameraTransform;
	private Vector3 initialOffset;

	[Range(-10f, 10f)] public float sensitivityX = 5f;
	[Range(-10f, 10f)] public float sensitivityY = -5f;
	//It's inverted from what you'd think for some reason. ¯\_(ツ)_/¯
	public float rotX = 0f;
	public float rotY = 0f;

	//Camera shake
	[Range(0f, 1f)] [SerializeField] private float trauma = 0f;
	[SerializeField] private float shakeSpeed = 15f;
	[SerializeField] private float traumaDecrement = 0.2f;

	private PlayerInputActionAsset inputActionAsset;
	private Vector2 cameraInput = Vector2.zero;

	void Start()
	{
		inputActionAsset = focus.GetComponent<PlayerController>().inputActionAsset;
		inputActionAsset.Movement.Camera.performed += context =>
		{
			cameraInput = context.ReadValue<Vector2>();
		};
		inputActionAsset.Movement.Camera.canceled += context =>
		{
			cameraInput = Vector2.zero;
		};

		try
		{
			cameraTransform = transform.GetChild(0);
			initialOffset = cameraTransform.localPosition;
			cameraTransform.GetComponent<Camera>();
		}
		catch
		{
			throw new System.Exception("Camera Controller does not have camera as child 0");
		}
	}

	void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, focus.position, lerpAmount);

		//rotX += Input.GetAxis("Mouse Y") * sensitivityY;  //Up is negative, Down is positive. ¯\_(ツ)_/¯
		//rotX = Mathf.Clamp(rotX, -90f, 90f);

		//rotY = rotY % 360f + (cameraInput.x * sensitivityX);
		transform.eulerAngles = transform.eulerAngles + new Vector3(0f, cameraInput.x * sensitivityX, 0f);

		//transform.rotation = Quaternion.Euler(rotX, rotY, 0f);

		DoCameraShake();
	}

	/// <summary> Trauma ranges from 0-1 </summary>
	public void AddShakeTrauma(float amount)
	{
		trauma = Mathf.Min(1f, trauma + amount);
	}

	private void DoCameraShake()
	{
		//ranges from -0.5 to 0.5
		float shakeX = Mathf.Pow(trauma, 2f) * (Mathf.PerlinNoise(69f, Time.time * shakeSpeed) - 0.5f);
		float shakeY = Mathf.Pow(trauma, 2f) * (Mathf.PerlinNoise(420f, Time.time * shakeSpeed) - 0.5f);
		float shakeZ = Mathf.Pow(trauma, 2f) * (Mathf.PerlinNoise(123f, Time.time * shakeSpeed) - 0.5f);

		cameraTransform.localPosition = new Vector3(shakeX, shakeY, shakeZ) + initialOffset;

		trauma = Mathf.Max(0f, trauma - (traumaDecrement * Time.timeScale * Time.deltaTime));
	}
}