using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SymptomEffect))]
public class SymptomEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        var affectedUnitProperty = serializedObject.FindProperty("AffectedUnit");
        EditorGUILayout.PropertyField(affectedUnitProperty);

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

                var Direction = serializedObject.FindProperty("KnockDirection");
                EditorGUILayout.PropertyField(Direction);

                var KnockIntensity = serializedObject.FindProperty("KnockbackIntensity");
                EditorGUILayout.PropertyField(KnockIntensity);

                var KnockCount = serializedObject.FindProperty("KnockbackCount");
                EditorGUILayout.PropertyField(KnockCount);

                var KnockInterval = serializedObject.FindProperty("KnockbackInterval");
                EditorGUILayout.PropertyField(KnockInterval);

                break;

            // Add cases for other enum values if needed

            case SymptomEffect.SymptomEffectType.DoT:
                var DotDamage = serializedObject.FindProperty("DotDamage");
                EditorGUILayout.PropertyField(DotDamage);

                var DotDuration = serializedObject.FindProperty("DotDuration");
                EditorGUILayout.PropertyField(DotDuration);

                var DotTickRate = serializedObject.FindProperty("DotTickRate");
                EditorGUILayout.PropertyField(DotTickRate);
                break;

            case SymptomEffect.SymptomEffectType.MoveSpeedModifier:
                var SpeedModifierAmount = serializedObject.FindProperty("MoveSpeedModifierAmount");
                EditorGUILayout.PropertyField(SpeedModifierAmount);

                var ModifierType = serializedObject.FindProperty("ModifierType");
                EditorGUILayout.PropertyField(ModifierType);

                var IsInfiniteDuration = serializedObject.FindProperty("IsInfiniteDuration");
                EditorGUILayout.PropertyField(IsInfiniteDuration);

                if (!IsInfiniteDuration.boolValue)
                {
                    var SpeedModifierDuration = serializedObject.FindProperty("MoveSpeedModifierDuration");
                    EditorGUILayout.PropertyField(SpeedModifierDuration);
                }
                break;

            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
