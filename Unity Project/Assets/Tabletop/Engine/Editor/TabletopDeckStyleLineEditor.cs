using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabletopDeckStyleLine), true)]
public class TabletopDeckStyleLineEditor : Deck2DStyleEditor
{
	SerializedProperty gap;
	SerializedProperty angle;
	SerializedProperty alignment;
	SerializedProperty cardsOrientation;
	SerializedProperty hasMaxNumberOfCards;
	SerializedProperty maxNumberOfCards;

	new void OnEnable()
	{
		base.OnEnable();
		gap = serializedObject.FindProperty("_gap");
		angle = serializedObject.FindProperty("_angle");
		alignment = serializedObject.FindProperty("alignment");
		cardsOrientation = serializedObject.FindProperty("cardsOrientation");
		hasMaxNumberOfCards = serializedObject.FindProperty("hasMaxNumberOfCards");
		maxNumberOfCards = serializedObject.FindProperty("_maxNumberOfCards");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		base.OnInspectorGUI();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Line", EditorStyles.boldLabel);

		// Gap
		gap.floatValue = Mathf.Max(0, EditorGUILayout.FloatField(new GUIContent("Gap", "The space between two cards."), gap.floatValue));

		// Angle
		angle.floatValue = EditorGUILayout.Slider(new GUIContent("Angle", "The angle of the line in degrees."), angle.floatValue, 0, 360f);

		// Alignment
		EditorGUILayout.PropertyField(alignment, new GUIContent("Alignment"));

		// Cards orientation
		EditorGUILayout.PropertyField(cardsOrientation, new GUIContent("Cards orientation"));

		// Max number of cards
		hasMaxNumberOfCards.boolValue = EditorGUILayout.Toggle("Visible cards limit (not working)", hasMaxNumberOfCards.boolValue);
		if(hasMaxNumberOfCards.boolValue)
		{
			EditorGUI.indentLevel++;
			maxNumberOfCards.intValue = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Visible cards"), maxNumberOfCards.intValue));
			EditorGUI.indentLevel--;
		}

		// TEST
		/*
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("~ Buttons test ~");
		GUILayout.Button("Uno", EditorStyles.miniButtonLeft);
		GUILayout.Button("Due", EditorStyles.miniButtonMid);
		GUILayout.Button("Tre", EditorStyles.miniButtonRight);
		EditorGUILayout.EndHorizontal();
		*/

		serializedObject.ApplyModifiedProperties();
	}
}
