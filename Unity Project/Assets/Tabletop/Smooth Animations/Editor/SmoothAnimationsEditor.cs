using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmoothAnimations)), CanEditMultipleObjects]
public class SmoothAnimationsEditor : Editor
{
	SerializedProperty smoothTranslationTime;
	SerializedProperty smoothTranslationAnimationCurve;
	SerializedProperty smoothRotationTime;
	SerializedProperty smoothRotationAnimationCurve;
	SerializedProperty smoothScaleTime;
	SerializedProperty smoothScaleAnimationCurve;

	protected void OnEnable()
	{
		smoothTranslationTime = serializedObject.FindProperty("_smoothTranslationTime");
		smoothTranslationAnimationCurve = serializedObject.FindProperty("smoothTranslationAnimationCurve");
		smoothRotationTime = serializedObject.FindProperty("_smoothRotationTime");
		smoothRotationAnimationCurve = serializedObject.FindProperty("smoothRotationAnimationCurve");
		smoothScaleTime = serializedObject.FindProperty("_smoothScaleTime");
		smoothScaleAnimationCurve = serializedObject.FindProperty("smoothScaleAnimationCurve");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Translation
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Translation", "Time and animation curve for smooth translation."));
		smoothTranslationTime.floatValue = Mathf.Max(0, EditorGUILayout.FloatField(smoothTranslationTime.floatValue));
		smoothTranslationAnimationCurve.animationCurveValue = EditorGUILayout.CurveField(smoothTranslationAnimationCurve.animationCurveValue);
		EditorGUILayout.EndHorizontal();

		// Rotation
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Rotation", "Time and animation curve for smooth rotation."));
		smoothRotationTime.floatValue = Mathf.Max(0, EditorGUILayout.FloatField(smoothRotationTime.floatValue));
		smoothRotationAnimationCurve.animationCurveValue = EditorGUILayout.CurveField(smoothRotationAnimationCurve.animationCurveValue);
		EditorGUILayout.EndHorizontal();

		// Scale
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Scale", "Time and animation curve for smooth scale."));
		smoothScaleTime.floatValue = Mathf.Max(0, EditorGUILayout.FloatField(smoothScaleTime.floatValue));
		smoothScaleAnimationCurve.animationCurveValue = EditorGUILayout.CurveField(smoothScaleAnimationCurve.animationCurveValue);
		EditorGUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}
