using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
#region [grey]
	
	Animator animator;
	int animSpeedHash = Animator.StringToHash("Speed");
	int animSpinHash = Animator.StringToHash("isSpin");

	[SerializeField] private Transform cameraFocus; //set in inspector because lazy

	CharacterController characterController;
	public PlayerInputActionAsset inputActionAsset;
	[HideInInspector] public Vector2 moveInput {get; private set;} = Vector2.zero; 	//most of these seem to be public just so they can be read by debug_ui
	public float acceleration = 50f;
	public Vector3 velocity = Vector3.zero; //why does adding any getter/setter not allow me to change components individually
	public float friction = 35f;
	public float gravity = -50f;
	private float maxInputVelXZ = 5f; //Only clamps velocity from inputs
	[HideInInspector] public Vector2 facing {get; private set;} = Vector2.zero;

	public PlayerHealth playerHealth {get; private set;}
	
#endregion

	void Awake()
	{
		inputActionAsset = new PlayerInputActionAsset();
		inputActionAsset.Movement.Enable();

		characterController = gameObject.GetComponent<CharacterController>();
		playerHealth = gameObject.GetComponent<PlayerHealth>();
		animator = gameObject.GetComponentInChildren<Animator>();

		//disable dialogue box
		
		if(acceleration < friction) Debug.LogWarning("moveAccel is less than friction. Character won't be able to move!");

#region Delegates for input
			inputActionAsset.Movement.Move.performed += context =>
			{
				moveInput = context.ReadValue<Vector2>();
				facing = context.ReadValue<Vector2>().normalized;
			};

			inputActionAsset.Movement.Move.canceled += context =>
			{
				moveInput = Vector2.zero;
			};

			inputActionAsset.Movement.Interact.started += context =>
			{
				foreach (Collider collider in Physics.OverlapBox(transform.position + Vector3.up, Vector3.one, transform.rotation))
				{
					if (collider.GetComponent<Interactable>() != null)
					{
						collider.GetComponent<Interactable>().Interact();
						break;
					}
				}
			};
#endregion
	}

	void Update()
	{
		float camRotY = Mathf.Deg2Rad * cameraFocus.transform.eulerAngles.y;
		Vector2 velocityInput = new Vector2(Mathf.Cos(camRotY) * moveInput.x + Mathf.Sin(camRotY) * moveInput.y,
											-Mathf.Sin(camRotY) * moveInput.x + Mathf.Cos(camRotY) * moveInput.y) * acceleration * Time.deltaTime; 
		Vector2 velocityXZ = new Vector2(velocity.x, velocity.z);

		if (characterController.isGrounded)
		{
			//find floor angle
			Physics.SphereCast(transform.position + (Vector3.up * characterController.height / 2f), characterController.radius, Vector3.down, out RaycastHit hit, characterController.height);
			float floorAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (floorAngle <= characterController.slopeLimit) //if floor angle < max slope, move if input
			{
				//Adds input to XZ velocity, up to maxInputVelXZ but keeps it the same if it goes over. Does nothing if no input, not worth checking for
				Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + velocityInput, Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
				velocity = new Vector3(newVelXZ.x, -1f, newVelXZ.y);

				//Apply friction and clamp movements
				Vector2 velXZ = new Vector2(velocity.x, velocity.z);
				velXZ = velXZ.normalized * Mathf.Max(velXZ.magnitude - (friction * Time.deltaTime), 0f);
				velocity = new Vector3(velXZ.x, Mathf.Clamp(velocity.y, -25, 20f), velXZ.y);
			}
			else //if slope too steep: apply gravity and slide, also move along slope
			{
				velocity += Vector3.ProjectOnPlane(Vector3.up * gravity * Time.deltaTime, hit.normal);
				velocityXZ = new Vector2(velocity.x, velocity.z);

				//Projects vectors until input is only side-to-side on the slope.
				Vector3 inputPerpendicularToPlane = Vector3.ProjectOnPlane(new Vector3(velocityInput.x, 0f, velocityInput.y), new Vector3(hit.normal.x, 0f, hit.normal.z));
				Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + new Vector2(inputPerpendicularToPlane.x, inputPerpendicularToPlane.z), Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
				velocity = new Vector3(newVelXZ.x, velocity.y, newVelXZ.y);

				//Apply friction and clamp movements but special
				Vector2 velXZ = new Vector2(velocity.x, velocity.z);
				Vector3 perpXZ = Vector3.ProjectOnPlane(new Vector3(velXZ.x, 0f, velXZ.y), new Vector3(hit.normal.x, 0f, hit.normal.z));
				Vector2 fricPerpXZ = perpXZ.normalized * Mathf.Min(friction - perpXZ.magnitude, perpXZ.magnitude) * Time.deltaTime;
				velocity = new Vector3(velocity.x - fricPerpXZ.x, Mathf.Clamp(velocity.y, -25, 20f), velocity.y - fricPerpXZ.y);
			}
		}
		else //not on ground, fall
		{
			velocity.y = velocity.y + (gravity * Time.deltaTime);

			Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + (velocityInput * 1f), Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
			velocity = new Vector3(newVelXZ.x, velocity.y, newVelXZ.y);

			//Apply friction and clamp movements
			Vector2 velXZ = new Vector2(velocity.x, velocity.z);
			velXZ = velXZ.normalized * Mathf.Max(velXZ.magnitude - (friction * Time.deltaTime), 0f);
			velocity = new Vector3(velXZ.x, Mathf.Clamp(velocity.y, -25, 20f), velXZ.y);
		}

		characterController.Move(velocity * Time.deltaTime);

		//Animations
		float animSpeed = animator.GetFloat(animSpeedHash);
		animator.SetFloat(animSpeedHash, new Vector2(velocity.x, velocity.z).magnitude/maxInputVelXZ);

		if (inputActionAsset.Movement.Attack.triggered) playerHealth.DealDamage();

		transform.rotation = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, facing), 0f);
	}

	public void SetUIControlMode(bool enable)
	{
		if (enable)
		{
			inputActionAsset.Movement.Disable();
			inputActionAsset.UI.Enable();
		}
		else
		{
			inputActionAsset.Movement.Enable();
			inputActionAsset.UI.Disable();
		}
	}
}
