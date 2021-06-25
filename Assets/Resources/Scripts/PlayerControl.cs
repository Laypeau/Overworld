/*

		Don't use this, it's bad

*/


using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerControl : MonoBehaviour
{
	public Transform cameraFocusTransform; //set in inspector
	private BoundedCamera cameraScript;
	private Rigidbody rigidBody;
	private CapsuleCollider capsuleCollider;

	private Animator animator;
	int animSpeedHash = Animator.StringToHash("Speed");

	#region Assorted
		[Header("Collider Settings")]
		[SerializeField] private float radius = 0.5f;
		[SerializeField] private float height = 2f;

		[Header("Movement Settings")]
		public float moveSpeed = 6f; //Units per second. Multiplied by Time.deltaTime when called.    Clamp to < radius
		[HideInInspector] public float yVel = 0f; //property. Check if the only reason this is public is because of the debug UI
		///////////////[SerializeField] private float maxAngle = 50f;
		[SerializeField] private float stairStepThreshold = 0.3f; //Clamp to >= 0
		[SerializeField] private float gravity = -0.5f; //make sure it's negative

		private bool _isOccupied = false;
		public bool isOccupied {get{return _isOccupied;}}

		private LayerMask terrainMask;
		private LayerMask interactMask;
	#endregion

	#region Controls
		//why are they public
		[HideInInspector] public Vector2 moveInput = Vector2.zero;
		/// <summary>Same as moveInput but doesn't return to (0,0) with no input</summary>
		private Vector2 moveDirInput = Vector2.zero;
		[HideInInspector] public InputState moveInputState = 0;
		[HideInInspector] public InputState jumpInputState = 0;
		[HideInInspector] public InputState interactInputState = 0;
		public enum InputState
		{
			/// <summary>Button is not pressed</summary>
			Cancelled = 0,
			/// <summary>This is the frame the button is pressed</summary>
			Started = 1,
			/// <summary>Button is pressed</summary>
			Performed = 2
		}
	#endregion

	void Start()
	{
		rigidBody = gameObject.GetComponent<Rigidbody>();
		capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
		try
		{
			cameraScript = cameraFocusTransform.GetComponent<BoundedCamera>();
		}
		catch
		{
			Debug.LogWarning("Unable to access BoundedCamera component on camera focus");
		}

		capsuleCollider.radius = radius;
		capsuleCollider.height = height;
		capsuleCollider.center = Vector3.up * height / 2;
		//throw error when radius > height/2  -- This makes it a sphere with a radius == variableRadius, which is > height

		animator = gameObject.GetComponent<Animator>();

		stairStepThreshold = Mathf.Max(stairStepThreshold, 0f);
		terrainMask = LayerMask.GetMask("Terrain");
		interactMask = Physics.AllLayers;//~LayerMask.GetMask("Terrain");
	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		Vector3 simulatedMove = Vector3.zero;
		float noClipping = 0.05f;
		Vector3 capBottom = transform.position + (Vector3.up * radius);
		Vector3 capTop = transform.position + (Vector3.up * (height - radius));

		Vector3 rawMove = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
		if (moveInputState != 0 && !isOccupied) rawMove = Quaternion.Euler(0f, cameraFocusTransform.rotation.eulerAngles.y, 0f) * new Vector3(moveInput.x, 0f, moveInput.y);

		bool sphereHit = Physics.SphereCast(capTop, radius, Vector3.down, out RaycastHit sphereRayhit, height + Mathf.Abs(yVel) + 3, terrainMask);
		if (sphereHit)
		{
			Debug.DrawRay(sphereRayhit.point, sphereRayhit.normal * 2f, new Color((Time.time + 1) % 2, (Time.time + 0.5f) % 2, Time.time % 2), 0.5f);
			bool refineHit = sphereRayhit.collider.Raycast(new Ray(sphereRayhit.point + Vector3.up, Vector3.down), out RaycastHit refineRayhit, 2f);
			float floorAngle = Vector3.Angle(sphereRayhit.normal, Vector3.up);
			float toleranceY = 0.1f;
			float deltaHitY = sphereRayhit.point.y - transform.position.y;

			if ((deltaHitY >= -toleranceY) && (deltaHitY <= radius + toleranceY))
			{
				yVel = 0;
				//place player on slope
				if (floorAngle <= 60f)
				{
					rawMove = Vector3.ProjectOnPlane(rawMove, sphereRayhit.normal);
					//Debug.Log("Projected move onto slope");
				}
				else
				{
					//slide, don't project move
					//Debug.Log("Slope too steep");
				}
			}
			else if (deltaHitY < -toleranceY)
			{
				yVel += gravity * Time.deltaTime;
				if (Physics.SphereCast(capBottom, radius, Vector3.down, out RaycastHit fallHit, yVel, terrainMask))
				{
					yVel = -fallHit.distance;
				}
			}
			else    //the character is inside a block and something broke
			{
				yVel = 0;

				if (deltaHitY < 1) //Whoops, inside block 
				{
					transform.position += new Vector3(0f, deltaHitY, 0f);
				}
			}
		}
		else
		{
			yVel += gravity * Time.deltaTime;
			if (Physics.SphereCast(capBottom, radius, Vector3.down, out RaycastHit fallHit, yVel, terrainMask))
			{
				yVel = -fallHit.distance;
			}
		}

		//WALL COLLISION
		//investigate better conditions
		//I can't image any scenario that needs more than two iterations, except when going into an acute corner, in which case one edge pushes you into the other edge, then back again and again
		//In the case of a wall on a slope, it hits the floor slope (0), hits the wall (1), then hits the floor again but projected onto the wall (2)
		//while has enough move left  &&  moving makes it hit wall  &&  can move in the first place
		float moveBudget = moveSpeed * Time.deltaTime;
		if (Physics.CapsuleCast(capBottom, capTop, radius, rawMove, out RaycastHit capsuleHit, moveBudget, terrainMask))    //if hits wall when move
		{
			float distToWall = Vector3.Distance(capsuleHit.point, capsuleCollider.ClosestPoint(capsuleHit.point));
			simulatedMove += rawMove * (distToWall - noClipping);
			moveBudget -= distToWall;

			int i = 0;
			while (i < 2)
			{
				Debug.Log($"Wall {i}: {Vector3.Angle(rawMove, Vector3.up)}");
				i++;
				Vector3 moveAgain = Vector3.zero;
				if (Vector3.Angle(rawMove, Vector3.ProjectOnPlane(rawMove, Vector3.up)) <= 60)
				{
					moveAgain = new Vector3(moveAgain.x, 0f, moveAgain.z);  //removes y component, so it doesn't climb unnecessary slopes
				}
				else
				{
					moveAgain = Vector3.ProjectOnPlane(rawMove, capsuleHit.normal);
				}
				//test if moveagain vector is big to check parallelness to wall
				//check if slope too big to remove  component
				if (Physics.CapsuleCast(capBottom, capTop, radius, moveAgain, out capsuleHit, moveBudget, terrainMask))
				{
					distToWall = Vector3.Distance(capsuleHit.point, capsuleCollider.ClosestPoint(capsuleHit.point));
					simulatedMove += moveAgain * (distToWall - noClipping);
					moveBudget -= distToWall;
					rawMove = moveAgain;
				}
				else
				{
					Debug.Log("break");
					simulatedMove += moveAgain * moveBudget;
					break;
				}
			}
		}
		else
		{
			simulatedMove += rawMove * moveSpeed * Time.deltaTime;
		}

		rigidBody.MovePosition(rigidBody.position + simulatedMove + (Vector3.up * yVel));

		//this is temporary.
		rigidBody.MoveRotation(Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, moveDirInput), 0f));

		//animation
		float animSpeed = animator.GetFloat(animSpeedHash);
		animator.SetFloat(animSpeedHash, moveInput.magnitude);
	}

	private void Interact()
	{
		if (isOccupied) {return;}
		Interactable interactingObject = null;
		float prevDist = 100f;
		foreach (Collider collider in Physics.OverlapBox(transform.position + new Vector3(moveInput.x, height/2f, moveInput.y), new Vector3(0.5f, height/2f, 0.5f), Quaternion.Euler(0f, 0f, 0f), interactMask))
		{
			if (collider.GetComponent<Interactable>() != null && Vector3.Distance(transform.position, collider.transform.position) < prevDist)
			{
				prevDist = Vector3.Distance(transform.position, collider.transform.position);
				interactingObject = collider.GetComponent<Interactable>();
			}
		}
		if (interactingObject != null) interactingObject.Interact();
	}

	public void SetOccupied(bool value)
	{
		if (value)
		{
			_isOccupied = true;
		}
		else
		{
			_isOccupied = false;
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		//Debug.Log($"{context.ReadValue<Vector2>()} phase {context.phase}");
		switch (context.phase)
		{
			case InputActionPhase.Canceled:
				moveInput = context.ReadValue<Vector2>().normalized;
				moveInputState = InputState.Cancelled;
				break;

			case InputActionPhase.Started:
				moveInput = context.ReadValue<Vector2>().normalized; //Documentation says not to do this because it wont necessarily reflect the controller state or something
				moveDirInput = context.ReadValue<Vector2>().normalized;// != Vector2.zero ? Vector2.zero: Vector2.zero;
				moveInputState = InputState.Started;
				break;

			case InputActionPhase.Performed:
				moveInput = context.ReadValue<Vector2>().normalized;
				moveDirInput = context.ReadValue<Vector2>().normalized;
				moveInputState = InputState.Performed;
				break;

			default:
				break;
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Canceled:
				jumpInputState = InputState.Cancelled;
				break;

			case InputActionPhase.Started:
				jumpInputState = InputState.Started;
				break;

			case InputActionPhase.Performed:
				jumpInputState = InputState.Performed;
				break;

			default:
				break;
		}
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		switch(context.phase)
		{
			case InputActionPhase.Canceled:
				interactInputState = InputState.Cancelled;
				break;

			case InputActionPhase.Started:
				interactInputState = InputState.Started;
				if (!isOccupied) Interact();
				break;

			case InputActionPhase.Performed:
				interactInputState = InputState.Performed;
				break;

			default:
				break;
		}
	}
}