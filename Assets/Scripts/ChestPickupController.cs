using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ChestPickupController : PickupBase
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Pickup(StatsController statsController)
    {
        ItemBase upgrade = FindAvailableUpgrade();
        Debug.Log("Chest Upgrade: " + upgrade);
        if (upgrade) upgrade.IncreaseLevel();
        KillSelf();
    }

    ItemBase FindAvailableUpgrade()
    {
        System.Random rng = new System.Random();
        // find any items attached to the player as they are potential upgrade targets
        List<ItemBase> availableItems = player.GetComponents<ItemBase>().ToList();
        // filter out any that don't have levels available
        List<ItemBase> filtered = availableItems.Where(item => item.levelUpgrades.Length > item.level).ToList();
        // remove any that are level 0
        List<ItemBase> onlyLeveled = availableItems.Where(item => item.level != 0).ToList();
        // shuffle them
        List<ItemBase> shuffled = onlyLeveled.OrderBy(item => rng.Next()).ToList();

        if (shuffled.Count == 0)
        {
            // no upgrades available
            return null;
        }

        return shuffled[0];
    }

    protected void KillSelf()
    {
        Destroy(gameObject);
    }
}
