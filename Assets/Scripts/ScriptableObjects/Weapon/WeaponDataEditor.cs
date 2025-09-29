using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WeaponData weaponData = (WeaponData)target;

        EditorGUILayout.LabelField("General Information", EditorStyles.boldLabel);
        weaponData.weaponName = EditorGUILayout.TextField("Weapon Name", weaponData.weaponName);
        weaponData.weaponType = (WeaponData.WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponData.weaponType);
        weaponData.icon = (Sprite)EditorGUILayout.ObjectField("Icon", weaponData.icon, typeof(Sprite), false);
        weaponData.weaponPrefab = (GameObject)EditorGUILayout.ObjectField("Weapon Prefab", weaponData.weaponPrefab, typeof(GameObject), false);

        switch (weaponData.weaponType)
        {
            case WeaponData.WeaponType.Firearm:
                DrawFirearmSettings(weaponData);
                break;
            case WeaponData.WeaponType.Melee:
                DrawMeleeSettings(weaponData);
                break;
            case WeaponData.WeaponType.Throwable:
                DrawThrowableSettings(weaponData);
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio & Effects", EditorStyles.boldLabel);
        weaponData.attackSound = (AudioClip)EditorGUILayout.ObjectField("Attack Sound", weaponData.attackSound, typeof(AudioClip), false);
        weaponData.attackEffect = (ParticleSystem)EditorGUILayout.ObjectField("Attack Effect", weaponData.attackEffect, typeof(ParticleSystem), false);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(weaponData);
        }
    }

    private void DrawFirearmSettings(WeaponData weaponData)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Firearm Settings", EditorStyles.boldLabel);
        weaponData.bulletPrefab = (GameObject)EditorGUILayout.ObjectField("Bullet Prefab", weaponData.bulletPrefab, typeof(GameObject), false);
        weaponData.fireRate = EditorGUILayout.FloatField("Fire Rate", weaponData.fireRate);
        weaponData.ammoCapacity = EditorGUILayout.IntField("Ammo Capacity", weaponData.ammoCapacity);
        weaponData.ammoType = (WeaponData.AmmoType)EditorGUILayout.EnumPopup("Ammo Type", weaponData.ammoType);
        weaponData.reloadTime = EditorGUILayout.FloatField("Reload Time", weaponData.reloadTime);
        weaponData.pelletCount = EditorGUILayout.IntField("Pellet Count", weaponData.pelletCount);
        weaponData.spread = EditorGUILayout.FloatField("Spread", weaponData.spread);
        weaponData.unloaded = EditorGUILayout.Toggle("Unloaded?", weaponData.unloaded);
    }

    private void DrawMeleeSettings(WeaponData weaponData)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Melee Settings", EditorStyles.boldLabel);
        weaponData.attackRange = EditorGUILayout.FloatField("Attack Range", weaponData.attackRange);
        weaponData.attackSpeed = EditorGUILayout.FloatField("Attack Speed", weaponData.attackSpeed);
        weaponData.attackDamage = EditorGUILayout.IntField("Damage", weaponData.attackDamage);
    }

    private void DrawThrowableSettings(WeaponData weaponData)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Throwable Settings", EditorStyles.boldLabel);
        weaponData.throwForce = EditorGUILayout.FloatField("Throw Force", weaponData.throwForce);
        weaponData.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", weaponData.explosionRadius);
        weaponData.explosionDamage = EditorGUILayout.FloatField("Explosion Damage", weaponData.explosionDamage);
        weaponData.detonatesOnImpact = EditorGUILayout.Toggle("Detonates On Impact", weaponData.detonatesOnImpact);
    }
}
