using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
	public float health {get; protected set;} = 5f;
	public float maxHealth {get; protected set;} = 10;
	public bool isInvulnerable {get; protected set;} = false; //Use this instead of toggle for isDead
	public bool isDead {get; protected set;} = false;
	public float invulnerabilityTime { get; protected set; } = 0.2f; //invulnerability on hit
	protected LayerMask entitiesMask;
	protected LayerMask playerMask;

	protected virtual void Awake()
	{
		entitiesMask = LayerMask.NameToLayer("Entities");
		playerMask = LayerMask.NameToLayer("Player");
	}

	public virtual void DealDamage()
	{

	}

	public virtual void TakeDamage(float value)
	{
		DecrementHealth(value);
		StartCoroutine(BecomeInvulnerable(invulnerabilityTime));
	}

	protected virtual void Die()
	{
		Debug.Log($"{gameObject} is dead");
		isDead = true;
		
	}

	///<summary> Decreases health to 0, calls Die() when that happens </summary>
	public virtual void DecrementHealth(float value)
	{
		if (!isInvulnerable && !isDead)
		{
			if (health - value <= 0f)
			{
				health = 0f;
				Die();
			}
			else
			{
				health -= value;
			}
		}
	}

	///<summary> Increases health up to maxHealth </summary>
	public virtual void IncreaseHealth(float value)
	{
		if (health + value > maxHealth)
		{
			health = maxHealth;
		}
		else
		{
			health += value;
		}
	}

	protected IEnumerator BecomeInvulnerable(float time)
	{
		isInvulnerable = true;
		yield return new WaitForSeconds(time);
		isInvulnerable = false;
	}
}