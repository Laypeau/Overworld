using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue Data")]
public class DialogueData : ScriptableObject
{
	public new string name; //does nothing for now

	[TextArea(4, 15)]
	public List<string> textBoxes;
}