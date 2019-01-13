using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabletopDeckStyle), true), CanEditMultipleObjects]
public class Deck2DStyleEditor : Editor
{
	SerializedProperty positionScatter;
	SerializedProperty angleScatter;

	protected void OnEnable()
	{
		positionScatter = serializedObject.FindProperty("_positionScatter");
		angleScatter = serializedObject.FindProperty("_angleScatter");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Scatter", EditorStyles.boldLabel);

		// Position scatter
		positionScatter.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Position", positionScatter.floatValue));

		// Angle scatter
		angleScatter.floatValue = EditorGUILayout.Slider("Angle", angleScatter.floatValue, 0, 180f);

		serializedObject.ApplyModifiedProperties();
	}
}