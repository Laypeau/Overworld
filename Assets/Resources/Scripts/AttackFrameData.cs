using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a list of hitboxes
[CreateAssetMenu(fileName = "New Attack Frame Data", menuName = "Attack Frame Data")]
public class AttackFrameData : ScriptableObject
{
	[Tooltip("How many FixedUpdate frames these hitboxes should last")] public float attackLength;
	[SerializeField] public List<AttackHitbox> attackHitboxes = new List<AttackHitbox>();
	void OnDrawGizmos()
	{

	}
}

public struct AttackHitbox
{
	public string name;
	public float damage;
	public Vector3 position;
	public Vector3 direction;
	public Vector3 rotation;
	public Vector3 dimensions;

	public AttackHitbox(string nm)
	{
		name = nm;
		damage = 1;
		position = Vector3.zero;
		direction = Vector3.zero;
		rotation = Vector3.zero;
		dimensions = Vector3.one;
	}

	public AttackHitbox(string nm, float dmg, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 dim)
	{
		name = nm;
		damage = dmg;
		position = pos;
		direction = dir;
		rotation = rot;
		dimensions = dim;
	}

	public bool Equals(AttackHitbox that)
	{
		if (this.name == that.name && this.damage == that.damage && this.position == that.position && this.direction == that.direction && this.rotation == that.rotation && this.dimensions == that.dimensions) return true;
		else return false;
	}
}