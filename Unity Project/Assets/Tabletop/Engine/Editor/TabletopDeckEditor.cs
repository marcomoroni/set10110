using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabletopDeck)), CanEditMultipleObjects]
public class TabletopDeckEditor : Editor
{
	SerializedProperty style;

	SerializedProperty gizmoCardsCount;
	SerializedProperty showCardGizmos;

	private void OnEnable()
	{
		style = serializedObject.FindProperty("style");

		gizmoCardsCount = serializedObject.FindProperty("gizmoCardsCount");
		showCardGizmos = serializedObject.FindProperty("showCardGizmos");
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		serializedObject.Update();

		// Style
		EditorGUILayout.ObjectField(style, new GUIContent("Style"));

		// Rules
		// (can add on top, bottom, etc)

		EditorGUILayout.Space();

		// Available in play mode only
		EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
		TabletopDeck script = (TabletopDeck)target;
		EditorGUILayout.BeginHorizontal();
		// Shuffle
		if (GUILayout.Button("Shuffle"))
		{
			script.Shuffle();
		}
		// Lay down
		if (GUILayout.Button("Lay down"))
		{
			script.LayDown();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		// Gizmos
		EditorGUILayout.Toggle("Gizmos", showCardGizmos.boolValue);
		EditorGUI.indentLevel++;
		gizmoCardsCount.intValue = Mathf.Max(0, EditorGUILayout.IntField("Cards", gizmoCardsCount.intValue));
		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();
	}
}
