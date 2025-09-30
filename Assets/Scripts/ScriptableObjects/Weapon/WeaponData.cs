using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    // --- ENUMS ---

    public enum WeaponType
    {
        Melee,      // Facón, Spear, Saber
        Firearm,    // Revolver, Carbine, Shotgun, Rifle
        Throwable,  // Boleadoras, Molotovs
        Environment // Fixed Cannons
    }

    public enum AmmoType
    {
        None,      // For Melee/Dedicated Throwables
        Pistol,    // Revolver, pistols
        Rifle,     // Carbine, Rifle
        Shotgun    // Shotguns
    }

    // --- GENERAL INFORMATION ---

    [Header("General Information")]
    public string weaponName;
    public WeaponType weaponType;
    public Sprite icon;
    [Tooltip("Prefab of the physical item that appears in the world and is equipped.")]
    public GameObject weaponPrefab;

    // --- POOL INTEGRATION ---

    [Header("Pool Integration")]
    [Tooltip("The category key for fetching the projectile from the ObjectPool.")]
    public string projectileCategory = "Projectiles";
    [Tooltip("The pool name key for fetching the projectile.")]
    public string projectilePoolName = "BasicBullet";

    [Tooltip("The category key for fetching an effect (e.g., impact, flash).")]
    public string effectCategory = "Effects";
    [Tooltip("The pool name key for fetching an effect.")]
    public string effectPoolName = "MuzzleFlash";

    // --- WEAPON MECHANICS (Common to ALL types) ---

    [Header("Weapon Mechanics (Throwing)")]
    [Tooltip("Base force applied when the player throws the weapon.")]
    public float throwForce = 15f;
    [Tooltip("The non-lethal damage dealt to an enemy upon being hit by the thrown weapon (used for stunning).")]
    public float thrownImpactDamage = 0.5f;

    [Header("Weapon Mechanics (Gaucho Special)")]
    // Propiedades 'limitedUses' y 'totalUses' ELIMINADAS.
    // El tipo 'Throwable' implica que el uso es limitado (un solo uso).

    // --- TYPE-SPECIFIC SETTINGS ---

    [Header("A. Melee Settings")]
    [Tooltip("Damage of the melee attack (1 is typically One-Hit-Kill in this system).")]
    public float attackDamage = 1f;
    public float attackRange = 1.5f;
    public float attackSpeed = 0.5f;
    [Tooltip("Can hit multiple enemies aligned (e.g., Lanza).")]
    public bool hasPenetration = false;

    [Header("B. Firearm Settings")]
    public float projectileDamage = 1f; // Damage applied by the pooled projectile
    public float fireRate = 0.1f;
    public AmmoType ammoType;
    public float reloadTime = 2f;
    [Tooltip("Number of projectiles fired per trigger pull (pellets).")]
    public int pelletCount = 1;
    public float spread = 0f;

    [Header("Firearm Initial State")]
    public int ammoCapacity = 6;
    [Tooltip("If true, the weapon spawns on the ground empty/unloaded.")]
    public bool unloaded = false;
    [Tooltip("The initial ammo count if 'unloaded' is false.")]
    public int initialAmmo = 6;

    [Header("C. Throwable/Explosive Settings")]
    [Tooltip("If the throwable affects an area (e.g., Molotov, Boleadoras).")]
    public bool isAreaEffect = false;
    public float areaRadius = 3f;
    public float areaDamage = 50f;
    [Tooltip("Leaves a persistent damage area (e.g., Molotov fire).")]
    public bool leavesPersistentEffect = false;
    [Tooltip("Extra stun time applied by this throwable's impact (e.g., for Boleadoras).")]
    public float additionalStunTime = 0.5f;


    // --- AUDIO & VISUALS ---

    [Header("Audio")]
    public AudioClip attackSound; // Fire, melee swing, or throw
    public AudioClip reloadSound;
}