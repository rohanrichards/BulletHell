using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStatBase : ItemBase
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
        foreach (GlobalStatUpgradeSO upgrade in upgradePool)
        {
            GlobalStatUpgradeSO instance = Instantiate<GlobalStatUpgradeSO>(upgrade);
            gameObject.GetComponent<StatsController>().availableUpgrades.Add(instance);
        }
    }

    public override void IncreaseLevel()
    {
        base.IncreaseLevel();
        GlobalStatUpgradeSO levelUpgrade = (GlobalStatUpgradeSO)levelUpgrades[level - 1];
        StatsController controller = GameObject.Find("PlayerScripts").GetComponent<StatsController>();
        levelUpgrade.ApplyUpgrade(controller);
    }
}
