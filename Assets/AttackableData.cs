using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attackable Data", menuName = "Attackable Data")]
public class AttackableData : ScriptableObject
{
	public float maxHealth;
	public float currentHealth;
	public float invulnerabilityTime;
}
