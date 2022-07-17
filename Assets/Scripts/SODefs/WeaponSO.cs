using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "SO's/Configs/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    [Header("Weapon Configs")]
    [Tooltip("Expressed as rounds per second")]
    public float rof = 1.0f;
    public float rofPercentBonus = 0f;
    public float rofFlatBonus = 0f;
    public float ROF
    {
        get { 
            float localROF = rof + rofFlatBonus + rof * (rofPercentBonus / 100);
            float globalROF = localROF + localROF * ((ECSPlayerController.stats.globalStatsConfig.rofPercentBonus / 100));
            return globalROF; 
        }
    }
    public int projectileCount = 1;
    public int projectileCountFlatBonus = 0;
    public int projectileCountPercentBonus = 0;
    public float ProjectileCount
    {
        get {
            int localProjectileCount = projectileCount + projectileCountFlatBonus;
            localProjectileCount = Mathf.CeilToInt(localProjectileCount + localProjectileCount * (projectileCountPercentBonus / 100));
            float globalProjectileCount = localProjectileCount + ECSPlayerController.stats.globalStatsConfig.projectileCountBonus;
            return globalProjectileCount;
        }
    }
    [Tooltip("Degrees. Used to calculate arc/bullet spread patterns")]
    public float baseSpread = 0f;
    public float spreadPercentBonus = 0f;
    public float Spread
    {
        get
        {
            float localSpread = baseSpread + (baseSpread * (spreadPercentBonus / 100));
            float globalSpread = localSpread + localSpread * (ECSPlayerController.stats.globalStatsConfig.spreadPercentBonus / 100);
            return globalSpread;
        }
    }

    [Header("Bullet Configs")]
    public float baseDamage = 1f;
    public float damageFlatBonus = 0f;
    public float damagePercentBonus = 0f;
    public float Damage
    {
        get
        {
            float localDamage = baseDamage + damageFlatBonus + (baseDamage * (damagePercentBonus / 100));
            float globalDamage = localDamage + localDamage * (ECSPlayerController.stats.globalStatsConfig.damagePercentBonus / 100);
            return globalDamage;
        }
    }
    public float baseLifespan = 0.25f;
    public float lifespanFlatBonus = 0f;
    public float lifespanPercentBonus = 0f;
    public float Lifespan
    {
        get
        {
            return baseLifespan + lifespanFlatBonus + (baseLifespan * (lifespanPercentBonus / 100));
        }
    }
    public float baseAOE = 1f;
    public float aoePercentBonus = 0f;
    public float AOE
    {
        get
        {
            float localAOE = baseAOE + (baseAOE * (aoePercentBonus / 100));
            float globalAOE = localAOE + localAOE * (ECSPlayerController.stats.globalStatsConfig.areaPercentBonus / 100);
            return globalAOE;
        }
    }
    public float baseKnockbackForce = 10;
    public float knockBackForcePercentBonus = 0;
    public float KnockBackForce
    {

        get {
            float localKnockback = baseKnockbackForce + baseKnockbackForce * (knockBackForcePercentBonus / 100);
            float globalKnockback = localKnockback + localKnockback * (ECSPlayerController.stats.globalStatsConfig.knockbackPercentBonus / 100);
            return globalKnockback;
        }
    }
    public float baseSpeed = 50f;
    public float Speed
    {
        get
        {
            return baseSpeed;
        }
    }
}
