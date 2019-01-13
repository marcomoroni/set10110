using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabletopCard)), CanEditMultipleObjects]
public class TabletopCardEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		/*TabletopCard script = (TabletopCard)target;

		EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
		script.faceUp = EditorGUILayout.Toggle("Face Up", script.faceUp);
		EditorGUI.EndDisabledGroup();*/
	}
}
