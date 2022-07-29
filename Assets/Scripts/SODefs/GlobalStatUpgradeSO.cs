using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Upgrade", menuName = "SO's/Global Upgrades/Level Up")]

public class GlobalStatUpgradeSO : UpgradeSO
{
    public float rof;
    public int projectileCount;
    public int projectilePierce;
    public float damage;
    public float knockback;
    public float aoe;
    public void ApplyUpgrade(StatsController statsController)
    {
        statsController.globalStatsConfig.rofPercentBonus += this.rof;
        statsController.globalStatsConfig.projectileCountBonus += this.projectileCount;
        statsController.globalStatsConfig.projectilePierceBonus += this.projectilePierce;
        statsController.globalStatsConfig.damagePercentBonus += this.damage;
        statsController.globalStatsConfig.areaPercentBonus += this.aoe;
        statsController.globalStatsConfig.knockbackPercentBonus += this.knockback;
    }
}
