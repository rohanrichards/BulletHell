using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public bool isUnlocked = false;
    public int level = 0;
    public int rarity = 5;

    // this is the upgrades the item will add into the available upgrades pool
    // probably something specific like "Shotgun Level Up" but may add others for synergy unlocks
    public UpgradeSO[] upgradePool;

    // this is the collection of upgrades applied at level up, one per level.
    public UpgradeSO[] levelUpgrades;

    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void IncreaseLevel()
    {
        level += 1;
    }
    public virtual void Unlock()
    {
        Debug.Log("Unlocking");
        isUnlocked = true;
    }
}
