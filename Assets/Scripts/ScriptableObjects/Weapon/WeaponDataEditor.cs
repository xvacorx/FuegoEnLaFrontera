using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    private SerializedProperty weaponTypeProp;
    private SerializedProperty ammoTypeProp;
    private SerializedProperty unloadedProp;
    private SerializedProperty isAreaEffectProp;

    // Campos Pool String (Resultados del auto-relleno y editables manualmente)
    private SerializedProperty projectileCategoryProp;
    private SerializedProperty projectilePoolNameProp;
    private SerializedProperty effectCategoryProp;
    private SerializedProperty effectPoolNameProp;

    // --- CAMPOS TEMPORALES NO SERIALIZADOS PARA ARRASTRE ---
    // Usados para capturar la referencia de un GameObject de la escena o Prefab.
    private Object _tempProjectileRef;
    private Object _tempEffectRef;
    // -----------------------------------------------------------------

    private void OnEnable()
    {
        // Cachear propiedades existentes
        weaponTypeProp = serializedObject.FindProperty("weaponType");
        ammoTypeProp = serializedObject.FindProperty("ammoType");
        unloadedProp = serializedObject.FindProperty("unloaded");
        isAreaEffectProp = serializedObject.FindProperty("isAreaEffect");

        // Cachear propiedades de String
        projectileCategoryProp = serializedObject.FindProperty("projectileCategory");
        projectilePoolNameProp = serializedObject.FindProperty("projectilePoolName");
        effectCategoryProp = serializedObject.FindProperty("effectCategory");
        effectPoolNameProp = serializedObject.FindProperty("effectPoolName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGeneralSection();
        DrawPoolIntegrationSection();
        DrawThrowingMechanics();
        DrawGauchoMechanics();

        CheckAndAdjustAmmoType();

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

        // 1. PROYECTIL
        DrawPoolReferenceFields("Projectile", ref _tempProjectileRef, projectileCategoryProp, projectilePoolNameProp);

        EditorGUILayout.Space(5);

        // 2. EFECTO
        DrawPoolReferenceFields("Effect", ref _tempEffectRef, effectCategoryProp, effectPoolNameProp);
    }

    private void DrawPoolReferenceFields(
        string label,
        ref Object tempRef,
        SerializedProperty categoryProp,
        SerializedProperty poolNameProp)
    {
        EditorGUILayout.LabelField($"{label} Pool References", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        // CAMPO TEMPORAL PARA ARRASTRE (DRAG HERE)
        EditorGUI.BeginChangeCheck();
        tempRef = EditorGUILayout.ObjectField(new GUIContent($"DRAG {label} HERE to Auto-Fill"), tempRef, typeof(GameObject), true);
        if (EditorGUI.EndChangeCheck() && tempRef != null)
        {
            // Si el objeto se arrastró, extraemos los nombres y los guardamos como strings
            UpdatePoolNames(tempRef as GameObject, categoryProp, poolNameProp);

            // Borrar la referencia temporal, ya que no se puede serializar al SO
            tempRef = null;
            // Aplicamos cambios inmediatamente para que se reflejen en los campos de string
            serializedObject.ApplyModifiedProperties();
            // Re-actualizamos el objeto serializado para el próximo dibujo
            serializedObject.Update();
        }

        // CAMPOS DE STRING (Editables manualmente)
        EditorGUILayout.PropertyField(categoryProp);
        EditorGUILayout.PropertyField(poolNameProp);

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Extrae el nombre del GameObject (Pool Name) y el nombre del padre (Category) 
    /// y actualiza las propiedades Serializadas.
    /// </summary>
    private void UpdatePoolNames(GameObject go, SerializedProperty categoryProp, SerializedProperty poolNameProp)
    {
        if (go != null)
        {
            // 1. Pool Name es el nombre del GameObject
            poolNameProp.stringValue = go.name;

            // 2. Category Name es el nombre del padre (si existe)
            if (go.transform.parent != null)
            {
                categoryProp.stringValue = go.transform.parent.name;
            }
            else
            {
                // Si no tiene padre (es raíz), usamos un valor por defecto
                categoryProp.stringValue = "General";
            }
        }
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

        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shotsPerSecond"));
        EditorGUILayout.PropertyField(ammoTypeProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reloadTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pelletCount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spread"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Firearm Initial State", EditorStyles.miniLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoCapacity"));
        EditorGUILayout.PropertyField(unloadedProp);

        if (!unloadedProp.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialAmmo"));
        }
    }

    private void DrawThrowableSettings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("C. Throwable/Explosive Settings", EditorStyles.boldLabel);

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