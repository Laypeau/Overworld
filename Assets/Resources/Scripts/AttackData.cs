using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An list of attackframes
[CreateAssetMenu(fileName = "New Attack Data", menuName = "Attack Data")]
public class AttackData : ScriptableObject
{
	public List<AttackFrameData> attackFrames;
}