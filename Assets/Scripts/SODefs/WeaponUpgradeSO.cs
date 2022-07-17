using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "SO's/Weapon Upgrades/Weapon Upgrade")]

public class WeaponUpgradeSO : UpgradeSO
{
    public float rof;
    public float damage;
    public int projectiles;
    public float knockback;
    public float lifespan;
    public float aoe;
    public float spread;
    [SerializeField]
    public FirePatternSO fireFunc;
    public virtual void ApplyUpgrade(WeaponBase target)
    {
        target.weaponConfig.rofPercentBonus += this.rof;
        target.weaponConfig.projectileCountFlatBonus += this.projectiles;
        target.weaponConfig.knockBackForcePercentBonus += this.knockback;
        target.weaponConfig.damagePercentBonus += this.damage;
        target.weaponConfig.lifespanPercentBonus += this.lifespan;
        target.weaponConfig.aoePercentBonus += this.aoe;
        target.weaponConfig.spreadPercentBonus += this.spread;
        if (this.fireFunc)
        {
            target.weaponConfig.FireFunc = this.fireFunc.Fire;
        }
    }
}
