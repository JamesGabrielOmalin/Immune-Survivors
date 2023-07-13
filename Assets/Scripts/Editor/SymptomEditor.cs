using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Symptom))]
public class SymptomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        var nameProperty = serializedObject.FindProperty("Name");
        EditorGUILayout.PropertyField(nameProperty);

        var enumProperty = serializedObject.FindProperty("EffectType");
        EditorGUILayout.PropertyField(enumProperty);


        var radiusProperty = serializedObject.FindProperty("SymptomRadius");
        EditorGUILayout.PropertyField(radiusProperty);

        var delayProperty = serializedObject.FindProperty("ActivationDelay");
        EditorGUILayout.PropertyField(delayProperty);

        var activationTypeProperty = serializedObject.FindProperty("ActivationType");
        EditorGUILayout.PropertyField(activationTypeProperty);

        var scriptableObject = target as Symptom;
        switch (scriptableObject.EffectType)
        {
            case Symptom.SymptomEffectType.Knockback:
                var value1Property = serializedObject.FindProperty("KnockbackIntensity");
                EditorGUILayout.PropertyField(value1Property);
                break;

            // Add cases for other enum values if needed

            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
