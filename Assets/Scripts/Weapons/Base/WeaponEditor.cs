using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon), true)]
public class WeaponEditor : Editor
{
    private Weapon targetWeapon;
    private SerializedProperty weaponDataProp;

    private SerializedProperty currentAmmoProp;
    private SerializedProperty isUnloadedProp;
    private Firearm targetFirearm;

    private void OnEnable()
    {
        targetWeapon = (Weapon)target;
        weaponDataProp = serializedObject.FindProperty("weaponData");

        targetFirearm = targetWeapon as Firearm;

        if (targetFirearm != null)
        {
            currentAmmoProp = serializedObject.FindProperty("_currentAmmo");
            isUnloadedProp = serializedObject.FindProperty("_isUnloaded");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(weaponDataProp);

        if (targetWeapon.weaponData == null)
        {
            EditorGUILayout.HelpBox("Asigna un WeaponData para ver los stats y el estado del arma.", MessageType.Warning);
            DrawDefaultInspectorExcluding("weaponData");
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawWeaponSummary(targetWeapon.weaponData);

        if (targetFirearm != null)
        {
            DrawFirearmLiveState(targetFirearm.weaponData);
        }

        DrawDefaultInspectorExcluding("weaponData");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWeaponSummary(WeaponData data)
    {
        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.cyan * 0.7f;
        EditorGUILayout.LabelField($"--- STATS: {data.weaponName} ({data.weaponType}) ---", EditorStyles.whiteLargeLabel);
        GUI.backgroundColor = Color.white;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponData"), new GUIContent("Scriptable Object"));

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("THROWING MECHANICS (Universal)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Thrown Impact Damage", data.thrownImpactDamage.ToString("F2"));
        EditorGUILayout.LabelField("Throw Force", data.throwForce.ToString("F1"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("TYPE-SPECIFIC STATS", EditorStyles.boldLabel);


        switch (data.weaponType)
        {
            case WeaponData.WeaponType.Melee:
                EditorGUILayout.LabelField("Attack Damage", data.attackDamage.ToString("F2"));
                EditorGUILayout.LabelField("Attack Range", data.attackRange.ToString("F2"));
                EditorGUILayout.LabelField("Attack Speed (APS)", data.attackSpeed.ToString("F2"));
                EditorGUILayout.LabelField("Has Penetration", data.hasPenetration.ToString());

                if (targetWeapon is MeleeWeapon meleeWeapon)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Enemy LayerMask", meleeWeapon.enemyLayer.value.ToString());
                }
                break;

            case WeaponData.WeaponType.Firearm:
                EditorGUILayout.LabelField("Projectile Damage", data.projectileDamage.ToString("F2"));

                EditorGUILayout.LabelField("Fire Rate (RPS)", data.shotsPerSecond.ToString("F2"));

                EditorGUILayout.LabelField("Ammo Type", data.ammoType.ToString());
                EditorGUILayout.LabelField("Capacity", data.ammoCapacity.ToString());
                break;

            case WeaponData.WeaponType.Throwable:
            case WeaponData.WeaponType.Environment:
                if (data.isAreaEffect)
                {
                    EditorGUILayout.LabelField("Area Damage", data.areaDamage.ToString("F2"));
                    EditorGUILayout.LabelField("Area Radius", data.areaRadius.ToString("F1"));
                    EditorGUILayout.LabelField("Persistent Effect", data.leavesPersistentEffect.ToString());
                }
                EditorGUILayout.LabelField("Additional Stun Time", data.additionalStunTime.ToString("F2"));
                break;
        }
    }

    private void DrawFirearmLiveState(WeaponData data)
    {
        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.yellow * 0.8f;
        EditorGUILayout.LabelField("--- FIREARM LIVE STATE (Instance) ---", EditorStyles.whiteLargeLabel);
        GUI.backgroundColor = Color.white;

        if (currentAmmoProp == null)
        {
            EditorGUILayout.HelpBox("Error de serialización: Asegúrate de que _currentAmmo y _isUnloaded tienen [SerializeField] en Firearm.cs.", MessageType.Error);
            return;
        }

        int currentAmmo = currentAmmoProp.intValue;
        int maxAmmo = data.ammoCapacity;

        string ammoDisplay = $"{currentAmmo} / {maxAmmo}";

        if (currentAmmo <= 0)
        {
            ammoDisplay = $"**EMPTY** / {maxAmmo}";
            EditorGUILayout.HelpBox("Current ammo is zero (0).", MessageType.Error);
        }

        EditorGUILayout.LabelField("Current Ammo", ammoDisplay, EditorStyles.boldLabel);

        if (isUnloadedProp.boolValue)
        {
            EditorGUILayout.HelpBox("Weapon initialized as UNLOADED.", MessageType.Info);
        }

        if (!targetFirearm.canBeReloaded)
        {
            EditorGUILayout.HelpBox("Reloading is permanently BLOCKED for this instance.", MessageType.Warning);
        }
    }

    private void DrawDefaultInspectorExcluding(string propertyToExclude)
    {
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("--- UNITY & COMPONENTS ---", EditorStyles.boldLabel);

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (iterator.name != propertyToExclude && iterator.name != "m_Script"
                && iterator.name != "_currentAmmo" && iterator.name != "_isUnloaded"
                && iterator.name != "firepoint")
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
}