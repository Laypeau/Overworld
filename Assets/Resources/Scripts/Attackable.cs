using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
	public AttackableData attackableData;
	public float currentHealth {get; protected set;} = 1f;
	public float maxHealth {get; protected set;} = 1;
	public bool isInvulnerable {get; protected set;} = false;
	public float invulnerabilityTime { get; protected set;} = 0.2f;
	protected LayerMask entitiesMask;
	protected LayerMask playerMask;

	protected virtual void Awake()
	{
		entitiesMask = 1<<LayerMask.NameToLayer("Entities");
		playerMask = 1<<LayerMask.NameToLayer("Player");

		if (attackableData != null)
		{
			currentHealth = attackableData.currentHealth;
			maxHealth = attackableData.maxHealth;
			invulnerabilityTime = attackableData.invulnerabilityTime;
		}
	}

	public virtual void DealDamage()
	{

	}

	public virtual void TakeDamage(float value)
	{
		if (!isInvulnerable)
		{
			currentHealth = Mathf.Max(0f, currentHealth - value);
			if (currentHealth <= 0) Die();
		}

		StartCoroutine(BecomeInvulnerable(invulnerabilityTime));
	}

	///<summary> Increases health up to maxHealth </summary>
	public virtual void IncreaseHealth(float value)
	{
		currentHealth = Mathf.Min(maxHealth, currentHealth + value);
	}

	protected IEnumerator BecomeInvulnerable(float time)
	{
		isInvulnerable = true;
		yield return new WaitForSeconds(time);
		isInvulnerable = false;
	}
	
	protected virtual void Die()
	{
		Debug.Log($"{gameObject} is dead");
		Destroy(this);	
	}
}