using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
	private float headOffset = 0.75f;

	private float cameraFocusPosLerpXZ = 1f;
	private float cameraFocusPosLerpY = 0.75f;

	[Range(-10f, 10f)] public float sensitivityX = 5f;
	[Range(-10f, 10f)] public float sensitivityY = -5f;

	//It's inverted from what you'd think for some reason. ¯\_(ツ)_/¯
	public float rotationX = 0f;
	public float rotationY = 0f;

	//Camera shake
	[Range(0f, 10f)] public float trauma = 0f;
	[HideInInspector] public float traumaDelta = 0f;
	private readonly float traumaDecrement = 0.15f;
	private float shakeSpeed = 10f;

	//Component references
	private GameObject player;
	private Transform cameraFocusTransform;
	private Transform cameraTransform;

	void Start()
	{
		Initialise();
	}

	void Update()
	{
		//AngleZoom();
		//CameraBackToCollider(CameraTransform);
	}

	void LateUpdate()
	{
		DoCameraShake();    //Put in lateupdate so any other script wanting to change the trauma value can do it in Update and have the camera rotate the same frame
	}

	public void Initialise()
	{
		player = GameObject.Find("Player");
		cameraFocusTransform = GetComponent<Transform>();
		cameraTransform = cameraFocusTransform.Find("Main Camera");
	}

	public void MoveToFocus(Transform _HeadPosition)
	{
		float _OutX = Mathf.Lerp(cameraFocusTransform.position.x, _HeadPosition.position.x, cameraFocusPosLerpXZ);
		float _OutY = Mathf.Lerp(cameraFocusTransform.position.y, _HeadPosition.position.y, cameraFocusPosLerpY);
		float _OutZ = Mathf.Lerp(cameraFocusTransform.position.z, _HeadPosition.position.z, cameraFocusPosLerpXZ);

		cameraFocusTransform.position = new Vector3(_OutX, _OutY, _OutZ);
	}

	public void RotateByMouse(Transform _FocusTransform)
	{
		//rotationX += Input.GetAxis("Mouse Y") * sensitivityY;  //Up is negative, Down is positive. ¯\_(ツ)_/¯
		//rotationX = Mathf.Clamp(rotationX, -90f, 90f);

		rotationY = rotationY % 360f + Input.GetAxis("Mouse X") * sensitivityX;

		_FocusTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
	}

	public void AddShakeTrauma(float _TraumaAmount)
	{
		trauma += _TraumaAmount;
	}

	/// <summary>
	/// Adds TraumaDelta to Trauma, clamped between 0 & 10 
	/// Shakes the camera exponentially proportional to Trauma, within the bounds of +-ShakeMaxZ. 
	/// Then it decrements Trauma by TraumaDecrement * Timescale.
	/// </summary>
	public void DoCameraShake()
	{
		trauma = Mathf.Clamp(trauma + traumaDelta, 0f, 10f);  //Probably remap it from 0-100
		traumaDelta = 0;

		//ranges from -0.5 to 0.5
		float _ShakeX = (Mathf.Pow(trauma, 2f) / 100) * (Mathf.PerlinNoise(69f, Time.time * shakeSpeed) - 0.5f);
		float _ShakeY = (Mathf.Pow(trauma, 2f) / 100) * (Mathf.PerlinNoise(420f, Time.time * shakeSpeed) - 0.5f);

		cameraTransform.localPosition = new Vector3(_ShakeX, _ShakeY, 0f);

		if (trauma - traumaDecrement * Time.timeScale <= 0)
		{
			trauma = 0;
		}
		else
		{
			trauma -= traumaDecrement * Time.timeScale;
		}
	}
}