using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "SO's/Configs/Bullet", order = 2)]
public class BulletSO : ScriptableObject
{
    public float baseDamage = 1f;
    public float damageFlatBonus = 0f;
    public float damagePercentBonus = 0f;
    public float Damage
    {
        get
        {
            return baseDamage + damageFlatBonus + (baseDamage * (damagePercentBonus / 100));
        }
    }

    public float baseSpeed = 50f;
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

    public float baseAOE = 0f;
    public float aoePercentBonus = 1f;
    public float AOE
    {
        get
        {
            return baseAOE + (baseAOE * (aoePercentBonus / 100));
        }
    }
}
