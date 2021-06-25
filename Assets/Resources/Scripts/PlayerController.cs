using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* 	
	Handles movement and interaction
*/

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
#region [grey]
	
	Animator animator;
	int animSpeedHash = Animator.StringToHash("Speed");
	int animSpinHash = Animator.StringToHash("isSpin");

	[SerializeField] private Transform cameraFocusTransform; //set in inspector because lazy

	#region Movement 
		CharacterController characterController;
		public PlayerInputActionAsset inputActionAsset;
		[HideInInspector] public Vector2 moveInput {get; private set;} = Vector2.zero;
		public float moveAccel = 50f;
		public Vector3 velocity = Vector3.zero; //access modifier? I think it's public only so that Debug_UI can read it
		public float friction = 35f;
		public float jumpSpeed = 10f;
		private float gravity = -50f;
		private float maxInputVelXZ = 5f; //Only clamps velocity from inputs
		private float absoluteMaxVelXZ = 20f; //Includes velocity from all sources
		private float minYVel = -25f;
		[Range(0f, 1f)] [SerializeField] private float normalJumpInfluence = 0.25f;
		[HideInInspector] public Vector2 facing {get; private set;}= Vector2.zero;
	#endregion

	public PlayerHealth playerHealth {get; private set;}
	
#endregion

	void OnEnable()
	{
		inputActionAsset.Movement.Enable(); //might mess with UI being in progress.
	}

	void Awake()
	{
		inputActionAsset = new PlayerInputActionAsset();

		characterController = gameObject.GetComponent<CharacterController>();
		playerHealth = gameObject.GetComponent<PlayerHealth>();
		animator = gameObject.GetComponentInChildren<Animator>();

		//disable dialogue box
		
		if(moveAccel < friction) Debug.LogWarning("moveAccel is less than friction. Character won't be able to move!");

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
		#region old
		/*
		if(characterController.isGrounded) 
		{
			Physics.SphereCast(transform.position + (Vector3.up * characterController.height / 2f), characterController.radius, Vector3.down, out RaycastHit hit, characterController.height);
			float angle = Vector3.Angle(hit.normal, Vector3.up); 
			
			if (angle <= characterController.slopeLimit)
			{
				velocity.y = inputActionAsset.Movement.Jump.triggered ? jumpSpeed : -4f;
				
				move = Vector3.ProjectOnPlane(move, hit.normal).normalized * move.magnitude;
				move += Vector3.ProjectOnPlane(new Vector3(moveInput.x, 0f, moveInput.y), hit.normal).normalized * moveAccel * Time.deltaTime;
			}
			else
			{
				if (inputActionAsset.Movement.Jump.triggered)
				{
					Vector3 angledJump = Vector3.Slerp(Vector3.up, hit.normal, normalJumpInfluence) * jumpSpeed;
					velocity = new Vector3(velocity.x + angledJump.x, angledJump.y, velocity.z + angledJump.z);
				}
				velocity += Vector3.ProjectOnPlane(Vector3.up * gravity * Time.deltaTime, hit.normal);
				velocity += -velocity * Time.deltaTime * 0.8f;

				move += new Vector3(moveInput.x, 0f, moveInput.y) * moveAccel * Time.deltaTime;
			}
		}
		else
		{
			float fall = velocity.y + (gravity * Time.deltaTime);
			velocity.y = (fall <= minYVel) ? minYVel : fall;

			move += new Vector3(moveInput.x, 0f, moveInput.y) * moveAccel * Time.deltaTime;
		}

		
		move = Vector3.ClampMagnitude(move, maxMoveSpeed);
		if (inputActionAsset.Movement.Move.phase == InputActionPhase.Waiting) move = (move.magnitude - (friction * Time.deltaTime) < 0f) ? Vector3.zero : move.normalized * (move.magnitude - (friction * Time.deltaTime));
		*/
		#endregion
		
		Vector2 velocityInput = moveInput * moveAccel * Time.deltaTime;
		bool fric = false;

		if (characterController.isGrounded)
		{
			Physics.SphereCast(transform.position + (Vector3.up * characterController.height / 2f), characterController.radius, Vector3.down, out RaycastHit hit, characterController.height);
			float floorAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (floorAngle <= characterController.slopeLimit) //if floor angle < max slope, jump and/or move depending in input
			{
				velocity.y = inputActionAsset.Movement.Jump.triggered ? jumpSpeed : -4f;

				//Increases velocity up to maxInputVelXZ, keeps it the same in has input and is over. What if no input?
				Vector2 velocityXZ = new Vector2(velocity.x, velocity.z);
				Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + (velocityInput * 1f), Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
				velocity = new Vector3(newVelXZ.x, velocity.y, newVelXZ.y);
fric = true;
			}
			else //if slope too steep: jump off floor normal, apply gravity and slide if applicable, move
			{
				if (inputActionAsset.Movement.Jump.triggered)
				{
					Vector3 angledJump = Vector3.Slerp(Vector3.up, hit.normal, normalJumpInfluence) * jumpSpeed;
					velocity = new Vector3(velocity.x + angledJump.x, angledJump.y, velocity.z + angledJump.z);
				}

				velocity += Vector3.ProjectOnPlane(Vector3.up * gravity * Time.deltaTime, hit.normal);

				//Projects vectors until input is only side-to-side on the slope. See "visualisation of slope vectors.blend"
				Vector2 velocityXZ = new Vector2(velocity.x, velocity.z);
				Vector3 asdf = Vector3.ProjectOnPlane(new Vector3(velocityInput.x, 0f, velocityInput.y), Vector3.ProjectOnPlane(hit.normal, Vector3.up));
				Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + new Vector2(asdf.x, asdf.z), Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
				velocity = new Vector3(newVelXZ.x, velocity.y, newVelXZ.y);

fric = false;
			}
		}
		else //not on ground, fall
		{
			velocity.y = Mathf.Max(minYVel, velocity.y + (gravity * Time.deltaTime));

			Vector2 velocityXZ = new Vector2(velocity.x, velocity.z);
			Vector2 newVelXZ = Vector2.ClampMagnitude(velocityXZ + (velocityInput * 1f), Mathf.Max(maxInputVelXZ, velocityXZ.magnitude));
			velocity = new Vector3(newVelXZ.x, velocity.y, newVelXZ.y);
fric = true;
		}

		//Apply friction and clamp movements
		Vector2 velXZ = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), 20f);
		if (fric) velXZ = velXZ.normalized * Mathf.Max(velXZ.magnitude - (friction * Time.deltaTime), 0f);
		velocity.y = Mathf.Clamp(velocity.y, -20f, 20f);
		velocity = new Vector3(velXZ.x, velocity.y, velXZ.y);

		characterController.Move(velocity * Time.deltaTime);

		//Animations
		float animSpeed = animator.GetFloat(animSpeedHash);
		animator.SetFloat(animSpeedHash, new Vector2(velocity.x, velocity.z).magnitude/maxInputVelXZ);
	if (inputActionAsset.Movement.Attack.ReadValue<float>() > 0.5f) StartCoroutine(SpinForSeconds(1f));
		transform.rotation = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, facing), 0f);
	}

	//it doesnt actually spin for seconds
	public IEnumerator SpinForSeconds(float duration)
	{
		animator.SetBool(animSpinHash, true);
		//yield return new WaitForSeconds(duration);
		while (inputActionAsset.Movement.Attack.ReadValue<float>() > 0.2f)
		{
			playerHealth.DealDamage();
			yield return null;
		}
		animator.SetBool(animSpinHash, false);
	}

	public void SetUIMode(bool enable)
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
