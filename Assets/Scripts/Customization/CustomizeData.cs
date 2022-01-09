using UnityEngine;
using redd096;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Zombie Punk/CustomizeData")]
public class CustomizeData : ScriptableObject
{
    public enum ECustomizationType { Add, Reduce }

    [Header("Menu")]
    public int Weight = 1;
    public string Name = "PowerUp";
    public string Description = "Description PowerUp";
    [ShowAssetPreview] public Sprite Sprite = default;

    [Header("Weapon Customization")]
    public bool SetAutomaticWeapon = false;
    [EnableIf("SetAutomaticWeapon")] [Tooltip("Keep pressed or click?")] public bool Automatic = false;
    [Space]
    public bool SetRateOfFire = false;
    [EnableIf("SetRateOfFire")] public ECustomizationType CustomizationTypeRateOfFire = ECustomizationType.Reduce;
    [EnableIf("SetRateOfFire")] [Tooltip("Delay between shots")] public float RateOfFire = 0.2f;
    [Space]
    public bool SetRecoil = false;
    [EnableIf("SetRecoil")] public ECustomizationType CustomizationTypeRecoil = ECustomizationType.Reduce;
    [EnableIf("SetRecoil")] [Tooltip("Push back when shoot")] public float Recoil = 1;
    [Space]
    public bool SetNoiseAccuracy = false;
    [EnableIf("SetNoiseAccuracy")] public ECustomizationType CustomizationTypeNoiseAccuracy = ECustomizationType.Reduce;
    [EnableIf("SetNoiseAccuracy")] [Tooltip("Rotate random the shot when instantiated")] public float NoiseAccuracy = 10;
    [Space]
    public bool SetBarrelSimultaneously = false;
    [EnableIf("SetBarrelSimultaneously")] [Tooltip("When more than one barrel, shoot every bullet simultaneously or from one random?")] public bool BarrelSimultaneously = true;
    
    [Header("Bullet Customization")]
    public bool SetBulletPrefab = false;
    [EnableIf("SetBulletPrefab")] public Bullet BulletPrefab = default;
    [Space]
    public bool SetDamage = false;
    [EnableIf("SetDamage")] public ECustomizationType CustomizationTypeDamage = ECustomizationType.Add;
    [EnableIf("SetDamage")] public float Damage = 10;
    [Space]
    public bool SetBulletSpeed = false;
    [EnableIf("SetBulletSpeed")] public ECustomizationType CustomizationTypeBulletSpeed = ECustomizationType.Add;
    [EnableIf("SetBulletSpeed")] public float BulletSpeed = 10;
    
    [Header("Ammo Customization")]
    public bool SetHasAmmo = false;
    [EnableIf("SetHasAmmo")] public bool HasAmmo = true;
    [Space]
    public bool SetMaxAmmo = false;
    [EnableIf("SetMaxAmmo")] public ECustomizationType CustomizationTypeMaxAmmo = ECustomizationType.Add;
    [EnableIf("SetMaxAmmo")] [Min(0)] public int MaxAmmo = 32;
    [Space]
    public bool SetDelayReload = false;
    [EnableIf("SetDelayReload")] public ECustomizationType CustomizationTypeDelayReload = ECustomizationType.Reduce;
    [EnableIf("SetDelayReload")] public float DelayReload = 1;

    /// <summary>
    /// Add this customization to weapon
    /// </summary>
    /// <param name="weaponRange"></param>
    /// <returns></returns>
    public WeaponRange AddRangeCustomizations(WeaponRange weaponRange)
    {
        //be sure weapon is not null
        if (weaponRange == null)
            return weaponRange;
        
        //add weapon customizations
        if (SetAutomaticWeapon)
            weaponRange.Automatic = Automatic;
        
        if(SetRateOfFire)
            weaponRange.RateOfFire += CustomizationTypeRateOfFire == ECustomizationType.Add ? RateOfFire : -RateOfFire;
        
        if(SetRecoil)
            weaponRange.Recoil += CustomizationTypeRecoil == ECustomizationType.Add ? Recoil : -Recoil;

        if (SetNoiseAccuracy)
            weaponRange.NoiseAccuracy += CustomizationTypeNoiseAccuracy == ECustomizationType.Add ? NoiseAccuracy : -NoiseAccuracy;

        if (SetBarrelSimultaneously)
            weaponRange.BarrelSimultaneously = BarrelSimultaneously;

        //add bullet customizations
        if (SetBulletPrefab)
            weaponRange.BulletPrefab = BulletPrefab;

        if (SetDamage)
            weaponRange.Damage += CustomizationTypeDamage == ECustomizationType.Add ? Damage : -Damage;

        if (SetBulletSpeed)
            weaponRange.BulletSpeed += CustomizationTypeBulletSpeed == ECustomizationType.Add ? BulletSpeed : -BulletSpeed;

        //add ammo customizations
        if (SetHasAmmo)
            weaponRange.hasAmmo = HasAmmo;

        if (SetMaxAmmo)
            weaponRange.maxAmmo += CustomizationTypeMaxAmmo == ECustomizationType.Add ? MaxAmmo : -MaxAmmo;

        if (SetDelayReload)
            weaponRange.delayReload += CustomizationTypeDelayReload == ECustomizationType.Add ? DelayReload : -DelayReload;

        return weaponRange;
    }
}
