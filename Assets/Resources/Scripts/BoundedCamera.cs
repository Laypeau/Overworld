using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(BoxCollider))]
public class BoundedCamera : MonoBehaviour
{
	public Transform following;
	private Transform cameraTransform;

	public Vector3 cameraFocusOffset = Vector3.zero;
	public Vector3 cameraOffset = new Vector3(0f, 5f, 5f);
	[Range(0f, 1f)] public float cameraLerp = 1f;

	LayerMask boundsMask;

	private float trauma;
	private float traumaDelta;
	[SerializeField] private float shakeX;
	[SerializeField] private float shakeY;

	void Start()
	{
		if (transform.GetChild(0).GetComponent<Camera>() == null) throw new UnityException("Camera not set as child 0 of camera focus");
		else cameraTransform = transform.GetChild(0).transform;

		if (following == null) Debug.LogWarning("BoundedCamera.following not set");

		boundsMask = LayerMask.GetMask("Camera");

		transform.position = following.position;
		cameraTransform.localPosition = cameraOffset;
	}

	void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, (following.position + cameraFocusOffset), cameraLerp); //make frame independent

		DoCameraShake();
	}

	/// <summary> Adds trauma. Clamped between 0f and 1f. </summary>
	/// <param name="traumaAmount"> the amount of trauma to add </param>
	public void AddShakeTrauma(float traumaAmount)
	{
		traumaDelta += traumaAmount;
	}

	private void DoCameraShake()
	{
		trauma = Mathf.Clamp(trauma + traumaDelta, 0f, 1f);
		traumaDelta = 0f;

		float x = Mathf.Pow(trauma, 2f) * (2f * Mathf.PerlinNoise(69f, Time.time * 5f) - 1f); //Increase the power trauma is raised to to increase the sharpness of the shake increase
		float y = Mathf.Pow(trauma, 2f) * (2f * Mathf.PerlinNoise(42f, Time.time * 5f) - 1f);
		cameraTransform.localPosition = cameraOffset + new Vector3(x, y, 0f);

		trauma -= (10f * Time.timeScale * Time.deltaTime);
		if (trauma - (10f * Time.timeScale * Time.deltaTime) < 0f) trauma = 0f;
	}
}
