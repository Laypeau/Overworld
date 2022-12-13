using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : Attackable
{
	PlayerController playerController;

	void Start()
	{
		playerController = gameObject.GetComponent<PlayerController>();
	}

	public override void DealDamage()
	{
		// ParticleSystem.ShapeModule asd = gameObject.GetComponent<ParticleSystem>().shape;
		// asd.position = new Vector3(playerController.facing.x, 1f, playerController.facing.y);
		gameObject.GetComponent<ParticleSystem>().Play();

		foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(1f, 1f, 1f), transform.rotation, entitiesMask))
		{
			Attackable attackable = collider.GetComponent<Attackable>();
			if (attackable != null) attackable.TakeDamage(1f);
		}
	}
}
 