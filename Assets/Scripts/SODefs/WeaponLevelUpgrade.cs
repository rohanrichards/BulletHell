using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "SO's/Weapon Upgrades/Level Up")]

public class WeaponLevelUpgrade : WeaponUpgradeSO
{
    public override void ApplyUpgrade(WeaponBase target)
    {
        target.weaponConfig.rofPercentBonus += this.rof;
        target.weaponConfig.projectileCountBonus += this.projectiles;
        target.weaponConfig.knockBackForcePercentBonus += this.knockback;
        target.bulletConfig.damagePercentBonus += this.damage;
        target.bulletConfig.lifespanPercentBonus += this.lifespan;
        target.bulletConfig.aoePercentBonus += this.aoe;
    }

    public override void ApplyUpgrade(ItemBase target)
    {
        throw new System.NotImplementedException();
    }
}
