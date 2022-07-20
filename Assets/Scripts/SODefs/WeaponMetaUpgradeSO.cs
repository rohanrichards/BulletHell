using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "SO's/Weapon Upgrades/Meta Upgrade")]

public class WeaponMetaUpgradeSO : WeaponUpgradeSO
{
    public int requiredLevel;
    public bool purchased = false;
    public bool applied = false;
    public int cost;
    public override void ApplyUpgrade(WeaponBase target)
    {
        if(target.level >= this.requiredLevel){
            base.ApplyUpgrade(target);
        }
    }
}
