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
        get { return rof + rofFlatBonus + rof * (rofPercentBonus / 100); }
    }
    public int projectileCount = 1;
    public int projectileCountBonus = 0;
    public float ProjectileCount
    {
        get { return projectileCount + projectileCountBonus; }
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
