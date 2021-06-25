using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControlEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}
