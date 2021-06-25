using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : Attackable
{
	private PlayerController player;
	
	protected override void Awake()
	{
		base.Awake();
		player = gameObject.GetComponent<PlayerController>();
	}

	public override void DealDamage()
	{
		foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(1f, 1f, 1f), transform.rotation, ~entitiesMask)) //dont hurt self
		{
			Attackable attackable = collider.GetComponent<Attackable>();
			if (attackable != null) attackable.TakeDamage(1f);
		}
	}

	protected override void Die()
	{
		Debug.Log("player is dead");
	}
}
