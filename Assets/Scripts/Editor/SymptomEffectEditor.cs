using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SymptomEffect))]
public class SymptomEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        //var nameProperty = serializedObject.FindProperty("Name");
        //EditorGUILayout.PropertyField(nameProperty);

        var enumProperty = serializedObject.FindProperty("EffectType");
        EditorGUILayout.PropertyField(enumProperty);

        var delayProperty = serializedObject.FindProperty("ActivationDelay");
        EditorGUILayout.PropertyField(delayProperty);

        var activationTypeProperty = serializedObject.FindProperty("ActivationType");
        EditorGUILayout.PropertyField(activationTypeProperty);

       var scriptableObject = target as SymptomEffect;
        switch (scriptableObject.EffectType)
        {
            case SymptomEffect.SymptomEffectType.Knockback:
                var value1Property = serializedObject.FindProperty("KnockbackIntensity");
                EditorGUILayout.PropertyField(value1Property);

                var Direction = serializedObject.FindProperty("Direction");
                EditorGUILayout.PropertyField(Direction);
                break;

            // Add cases for other enum values if needed

            case SymptomEffect.SymptomEffectType.DOT:
                var DotDamage = serializedObject.FindProperty("DotDamage");
                EditorGUILayout.PropertyField(DotDamage);

                var DotDuration = serializedObject.FindProperty("DotDuration");
                EditorGUILayout.PropertyField(DotDuration);

                var DotTickRate = serializedObject.FindProperty("DotTickRate");
                EditorGUILayout.PropertyField(DotTickRate);
                break;

            case SymptomEffect.SymptomEffectType.MoveSpeedBuff:
                var SpeedBuffAmount = serializedObject.FindProperty("MoveSpeedBuffAmount");
                EditorGUILayout.PropertyField(SpeedBuffAmount);

                var ModifierType = serializedObject.FindProperty("ModifierType");
                EditorGUILayout.PropertyField(ModifierType);

                var SpeedBuffDuration = serializedObject.FindProperty("MoveSpeedBuffDuration");
                EditorGUILayout.PropertyField(SpeedBuffDuration);
                break;

            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
