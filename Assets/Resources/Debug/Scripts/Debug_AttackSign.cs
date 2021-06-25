using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_AttackSign : Attackable
{
	
	private GameObject player; //////////////////////////////////////////////give reference to player object when interact
	private Transform topHalf; //set in inspector

	protected override void Awake()
	{
		base.Awake();
		health = 1;
		player = GameObject.Find("Player");
		topHalf = transform.GetChild(0);
	}

	protected override void Die()
	{
		base.Die();

		//interactDialogue.canInteract = false;
		topHalf.GetComponent<Rigidbody>().AddExplosionForce(1f, transform.position, 0f, 1.5f, ForceMode.VelocityChange);
	}
}
