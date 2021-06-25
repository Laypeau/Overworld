using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackFrameData))]
public class AttackFrameDataEditor : Editor 
{
	int selectedIndex = 0;
	private new string name = "";
	private float damage = 1;
	private Vector3 position = Vector3.zero;
	private Vector3 direction = Vector3.forward;
	private Vector3 rotation = Vector3.zero;
	private Vector3 dimensions = Vector3.one;
	
	public override void OnInspectorGUI()
	{
		AttackFrameData frameData = (AttackFrameData)target;

		#region
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Apply", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
		{
			frameData.attackHitboxes[selectedIndex] = new AttackHitbox(name, damage, position, direction, rotation, dimensions);
		}
		if (GUILayout.Button("New", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
		{
			frameData.attackHitboxes.Add(new AttackHitbox(name, damage, position, direction, rotation, dimensions));
		}
		if (GUILayout.Button("Delete", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
		{
			frameData.attackHitboxes.RemoveAt(selectedIndex); /////////////////////replace with a check that greys out the box if invalid or something
			if (selectedIndex + 1 > frameData.attackHitboxes.Count) selectedIndex = frameData.attackHitboxes.Count - 1;
		}
		if (GUILayout.Button("▲", GUILayout.Width(30), GUILayout.Height(30)))
		{
			if (selectedIndex != 0)
			{
				AttackHitbox temp = frameData.attackHitboxes[selectedIndex];
				frameData.attackHitboxes.RemoveAt(selectedIndex);
				frameData.attackHitboxes.Insert(selectedIndex - 1, temp);
				selectedIndex -= 1;
			}
		}
		if (GUILayout.Button("▼", GUILayout.Width(30), GUILayout.Height(30)))
		{
			if (selectedIndex != frameData.attackHitboxes.Count - 1)
			{
				AttackHitbox temp = frameData.attackHitboxes[selectedIndex];
				frameData.attackHitboxes.RemoveAt(selectedIndex);
				frameData.attackHitboxes.Insert(selectedIndex + 1, temp);
				selectedIndex += 1;
			}
		}
		GUILayout.EndHorizontal();
		#endregion

		GUILayout.Space(10);

		#region
		GUILayout.BeginVertical();
			#region [Name and Damage]
			GUILayout.BeginHorizontal();
				GUILayout.Label("Name", GUILayout.Width(75));
				name = GUILayout.TextField(name, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label("Damage", GUILayout.Width(75));
				string damageString = GUILayout.TextField(damage.ToString(), GUILayout.ExpandWidth(true));
				if (float.TryParse(damageString, out float damageParsed))	damage = damageParsed;
			GUILayout.EndHorizontal();
			#endregion

			#region [Position]
			GUILayout.BeginHorizontal();
				GUILayout.Label("Position", GUILayout.Width(75));
				
				GUILayout.BeginHorizontal();
					float posX;
					string posXString = GUILayout.TextField(position.x.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(posXString, out posX)) posX = position.x;
					
					float posY;
					string posYString = GUILayout.TextField(position.y.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(posYString, out posY)) posY = position.y;
					
					float posZ;
					string posZString = GUILayout.TextField(position.z.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(posZString, out posZ)) posZ = position.z;

					position = new Vector3(posX, posY, posZ);
				GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			#endregion

			#region [Direction]
			GUILayout.BeginHorizontal();
				GUILayout.Label("Direction", GUILayout.Width(75));
				
				GUILayout.BeginHorizontal();
					float dirX;
					string dirXString = GUILayout.TextField(direction.x.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dirXString, out dirX)) dirX = direction.x;
					
					float dirY;
					string dirYString = GUILayout.TextField(direction.y.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dirYString, out dirY)) dirY = direction.y;
					
					float dirZ;
					string dirZString = GUILayout.TextField(direction.z.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dirZString, out dirZ)) dirZ = direction.z;

					direction = new Vector3(dirX, dirY, dirZ);
				GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			#endregion

			#region [Rotation]
			GUILayout.BeginHorizontal();
				GUILayout.Label("Rotation", GUILayout.Width(75));
				
				GUILayout.BeginHorizontal();
					float rotX;
					string rotXString = GUILayout.TextField(rotation.x.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(rotXString, out rotX)) rotX = rotation.x;
					
					float rotY;
					string rotYString = GUILayout.TextField(rotation.y.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(rotYString, out rotY)) rotY = rotation.y;
					
					float rotZ;
					string rotZString = GUILayout.TextField(rotation.z.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(rotZString, out rotZ)) rotZ = rotation.z;

					rotation = new Vector3(rotX, rotY, rotZ);
				GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			#endregion

			#region [Dimensions]
			GUILayout.BeginHorizontal();
				GUILayout.Label("Dimensions", GUILayout.Width(75));
				
				GUILayout.BeginHorizontal();
					float dimX;
					string dimXString = GUILayout.TextField(dimensions.x.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dimXString, out dimX)) dimX = dimensions.x;
					
					float dimY;
					string dimYString = GUILayout.TextField(dimensions.y.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dimYString, out dimY)) dimY = position.y;
					
					float dimZ;
					string dimZString = GUILayout.TextField(dimensions.z.ToString(), GUILayout.ExpandWidth(true));
					if (!float.TryParse(dimZString, out dimZ)) dimZ = dimensions.z;

					dimensions = new Vector3(dimX, dimY, dimZ);
				GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			#endregion	
		GUILayout.EndVertical();
		#endregion
		
		GUILayout.Space(10);

		GUIContent[] gridContents = new GUIContent[frameData.attackHitboxes.Count];
		for(int i = 0; i < frameData.attackHitboxes.Count; i++)
		{
			gridContents[i] = new GUIContent();
			gridContents[i].text = frameData.attackHitboxes[i].name;
			gridContents[i].tooltip = $"Dmg:	{frameData.attackHitboxes[i].damage}\nPos:	{frameData.attackHitboxes[i].position}\nDir:	{frameData.attackHitboxes[i].direction}\nRot:	{frameData.attackHitboxes[i].rotation}\nDim:	{frameData.attackHitboxes[i].dimensions}";
		}
		int prevIndex = selectedIndex;
		selectedIndex = GUILayout.SelectionGrid(selectedIndex, gridContents, 1);
		
		if (selectedIndex != prevIndex)
			{
				name = frameData.attackHitboxes[selectedIndex].name;
				damage = frameData.attackHitboxes[selectedIndex].damage;
				position = frameData.attackHitboxes[selectedIndex].position;
				direction = frameData.attackHitboxes[selectedIndex].direction;
				rotation = frameData.attackHitboxes[selectedIndex].rotation;
				dimensions = frameData.attackHitboxes[selectedIndex].dimensions;
			}

	}

	private void OnDrawGizmos()
	{

	}
}
