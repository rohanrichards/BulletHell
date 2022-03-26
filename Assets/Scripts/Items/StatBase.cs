using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBase : ItemBase
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Unlock()
    {
        base.Unlock();
        //enable the weapon and add its upgrades into available upgrades
        foreach (StatUpgradeSO upgrade in upgradePool)
        {
            StatUpgradeSO instance = Instantiate<StatUpgradeSO>(upgrade);
            gameObject.GetComponent<StatsController>().availableUpgrades.Add(instance);
        }
    }

    public override void IncreaseLevel()
    {
        base.IncreaseLevel();
        StatUpgradeSO levelUpgrade = (StatUpgradeSO)levelUpgrades[level - 1];
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        StatsController controller = player.GetComponent<StatsController>();
        levelUpgrade.ApplyUpgrade(controller);
    }
}
