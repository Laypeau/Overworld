using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Debug_UI : MonoBehaviour
{
	public bool playerMoveVisible = true;

	public PlayerController player;
	private TMP_Text playerMoveInput;
	private TMP_Text playerMoveState;
	private TMP_Text playerIsOccupied;
	private TMP_Text playerYVel;
	private TMP_Text playerJumpState;
	private TMP_Text playerIsGrounded;
	private TMP_Text playerVelocity;
	private TMP_Text playerMove;
	private TMP_Text playerFacing;
	private TMP_Text playerUIEnabled;

	void Start()
    {
		player = GameObject.Find("Player").GetComponent<PlayerController>();
	
		playerMoveInput = transform.Find("Player Control/Move Input").GetComponent<TMP_Text>();
		playerMoveState = transform.Find("Player Control/Move State").GetComponent<TMP_Text>();
		playerIsOccupied = transform.Find("Player Control/Can Move").GetComponent<TMP_Text>();
		playerYVel = transform.Find("Player Control/YVel").GetComponent<TMP_Text>();
		playerJumpState = transform.Find("Player Control/Jump State").GetComponent<TMP_Text>();
		playerIsGrounded = transform.Find("Player Control/Grounded").GetComponent<TMP_Text>();
		playerVelocity = transform.Find("Player Control/Velocity").GetComponent<TMP_Text>();
		playerMove = transform.Find("Player Control/Move").GetComponent<TMP_Text>();
		playerFacing = transform.Find("Player Control/Facing").GetComponent<TMP_Text>();
		playerUIEnabled = transform.Find("Player Control/UI Enabled").GetComponent<TMP_Text>();
	}

	void LateUpdate()
	{
		if (playerMoveVisible)
		{
			playerMoveInput.text = player.moveInput.ToString();
			playerMoveState.text = player.inputActionAsset.Movement.Move.phase.ToString();
			playerIsOccupied.text = player.inputActionAsset.Movement.Attack.phase.ToString();// ? "attack" : "nope";
			playerYVel.text = player.velocity.y.ToString();
			playerJumpState.text = "N/A";
			playerIsGrounded.text = player.GetComponent<CharacterController>().isGrounded.ToString();
			playerVelocity.text = player.velocity.ToString();
			playerMove.text = "N/A";
			playerFacing.text = player.facing.ToString();
			playerUIEnabled.text = player.inputActionAsset.UI.enabled.ToString();
		}
	}
}
