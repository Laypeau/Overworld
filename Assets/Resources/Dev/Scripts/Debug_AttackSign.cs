using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_AttackSign : Attackable
{
	
	private GameObject player;
	private Transform topHalf; //set in inspector

	protected void Start()
	{
		topHalf = transform.GetChild(0);
	}

	protected override void Die()
	{
		//interactDialogue.canInteract = false;
		topHalf.GetComponent<Rigidbody>().AddExplosionForce(4f, transform.position, 0f, 1.5f, ForceMode.VelocityChange);
		
		Destroy(this);
	}
}
