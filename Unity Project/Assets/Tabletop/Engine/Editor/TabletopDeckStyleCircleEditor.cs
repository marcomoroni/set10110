using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabletopDeckStyleCircle), true)]
public class TabletopDeckStyleCircleEditor : Deck2DStyleEditor
{
	SerializedProperty radius;
	SerializedProperty startAngle;
	SerializedProperty alignment;
	SerializedProperty direction;
	SerializedProperty gap;
	SerializedProperty cardsOrientation;

	new void OnEnable()
	{
		base.OnEnable();
		radius = serializedObject.FindProperty("_radius");
		startAngle = serializedObject.FindProperty("_startAngle");
		alignment = serializedObject.FindProperty("alignment");
		direction = serializedObject.FindProperty("direction");
		gap = serializedObject.FindProperty("_gap");
		cardsOrientation = serializedObject.FindProperty("cardsOrientation");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		base.OnInspectorGUI();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Circle", EditorStyles.boldLabel);

		// Radius
		radius.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Radius", radius.floatValue));

		// Start angle
		startAngle.floatValue = EditorGUILayout.Slider(new GUIContent("Start angle", "Starting angle in degrees."), startAngle.floatValue, 0, 360f);

		// Alignment
		EditorGUILayout.PropertyField(alignment, new GUIContent("Alignment"));

		// Direction
		EditorGUILayout.PropertyField(direction, new GUIContent("Direction"));

		// Gap
		gap.floatValue = EditorGUILayout.Slider(new GUIContent("Gap", "The angle gap between the cards in degrees."), gap.floatValue, 0, 360f);

		// Cards orientation
		EditorGUILayout.PropertyField(cardsOrientation, new GUIContent("Cards orientation"));

		serializedObject.ApplyModifiedProperties();
	}
}
