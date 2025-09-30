using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    // Properties to cache for better performance and robust handling
    private SerializedProperty weaponTypeProp;
    private SerializedProperty ammoTypeProp;
    // private SerializedProperty limitedUsesProp; // ELIMINADO
    private SerializedProperty unloadedProp;
    private SerializedProperty isAreaEffectProp;

    private void OnEnable()
    {
        // Find and cache all properties needed for conditional logic and drawing
        weaponTypeProp = serializedObject.FindProperty("weaponType");
        ammoTypeProp = serializedObject.FindProperty("ammoType");
        // limitedUsesProp = serializedObject.FindProperty("limitedUses"); // ELIMINADO
        unloadedProp = serializedObject.FindProperty("unloaded");
        isAreaEffectProp = serializedObject.FindProperty("isAreaEffect");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1. Draw Global Sections
        DrawGeneralSection();
        DrawPoolIntegrationSection();
        DrawThrowingMechanics();
        DrawGauchoMechanics(); // <--- Ahora es una sección vacía pero se mantiene por estructura

        // Check if weaponType changed and auto-adjust AmmoType
        CheckAndAdjustAmmoType();

        // 2. Conditional Section (Type-Specific)
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Type-Specific Settings", EditorStyles.boldLabel);

        WeaponData.WeaponType currentType = (WeaponData.WeaponType)weaponTypeProp.enumValueIndex;

        switch (currentType)
        {
            case WeaponData.WeaponType.Melee:
                DrawMeleeSettings();
                break;
            case WeaponData.WeaponType.Firearm:
                DrawFirearmSettings();
                break;
            case WeaponData.WeaponType.Throwable:
                DrawThrowableSettings();
                break;
        }

        // 3. Draw Audio Section (Always visible)
        DrawAudioSection();

        serializedObject.ApplyModifiedProperties();
    }

    // --- Core Logic Methods ---

    private void CheckAndAdjustAmmoType()
    {
        WeaponData.WeaponType currentType = (WeaponData.WeaponType)weaponTypeProp.enumValueIndex;

        // Si es Melee, Throwable o Environment, el AmmoType debe ser None
        if (currentType == WeaponData.WeaponType.Melee ||
            currentType == WeaponData.WeaponType.Throwable ||
            currentType == WeaponData.WeaponType.Environment)
        {
            if (ammoTypeProp.enumValueIndex != (int)WeaponData.AmmoType.None)
            {
                ammoTypeProp.enumValueIndex = (int)WeaponData.AmmoType.None;
            }
        }
    }

    // --- Drawing Methods ---

    private void DrawGeneralSection()
    {
        EditorGUILayout.LabelField("General Information", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponName"));
        EditorGUILayout.PropertyField(weaponTypeProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponPrefab"));
    }

    private void DrawPoolIntegrationSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pool Integration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectilePoolName"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("effectCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effectPoolName"));
    }

    private void DrawThrowingMechanics()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Mechanics (Throwing)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("throwForce"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("thrownImpactDamage"));
    }

    private void DrawGauchoMechanics()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Mechanics (Gaucho Special)", EditorStyles.boldLabel);

        // Sin contenido aquí, ya que 'alwaysAutoRecover' y 'limitedUses' fueron eliminados.
    }

    private void DrawMeleeSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("A. Melee Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasPenetration"));
    }

    private void DrawFirearmSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("B. Firearm Settings", EditorStyles.boldLabel);

        // Performance
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
        EditorGUILayout.PropertyField(ammoTypeProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reloadTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pelletCount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spread"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Firearm Initial State", EditorStyles.miniLabel);

        // State
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCapacity"));
        EditorGUILayout.PropertyField(unloadedProp);

        // Only show initialAmmo if the weapon isn't forced unloaded
        if (!unloadedProp.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialAmmo"));
        }
    }

    private void DrawThrowableSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("C. Throwable/Explosive Settings", EditorStyles.boldLabel);

        // Area Effect Logic (e.g., Molotov, Boleadoras)
        EditorGUILayout.PropertyField(isAreaEffectProp);
        if (isAreaEffectProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaRadius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leavesPersistentEffect"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("additionalStunTime"));
    }

    private void DrawAudioSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reloadSound"));
    }
}