using System.Collections;
using UnityEngine;
using TMPro;

public abstract class Interactable : MonoBehaviour
{
	private protected PlayerController player;

	protected virtual void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerController>(); ///////////////////////////////////////surely there's a better way to do this
	}

	public abstract void Interact();
}