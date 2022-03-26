using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Upgrade", menuName = "SO's/Stat Upgrades/Level Up")]

public class StatUpgrade : StatUpgradeSO
{
    public override void ApplyUpgrade(StatsController statsController)
    {
        statsController.statsConfig.healthPercentBonus += this.health;
        statsController.statsConfig.moveSpeedPercentBonus += this.movement;
        statsController.statsConfig.rotateSpeedPercentBonus += this.rotation;
        statsController.statsConfig.XPPercentBonus += this.xp;

        // Increase the current health by the current percent bonus amount
        float healthBonus = statsController.statsConfig.baseHealth * (this.health/100);
        statsController.statsConfig.currentHealth += Mathf.CeilToInt(healthBonus);

    }

    public override void ApplyUpgrade(ItemBase target)
    {
        throw new System.NotImplementedException();
    }
}
