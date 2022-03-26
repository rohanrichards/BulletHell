using UnityEngine;

public abstract class WeaponUpgradeSO : UpgradeSO
{
    public float rof;
    public float damage;
    public int projectiles;
    public float knockback;
    public float lifespan;
    public float aoe;
    public abstract void ApplyUpgrade(WeaponBase target);
}

public abstract class GlobalStatUpgradeSO : UpgradeSO
{
    public float rof;
    public int projectileCount;
    public int projectilePierce;
    public float damage;
    public float knockback;
    public float aoe;
    public abstract void ApplyUpgrade(StatsController statsController);
}

public abstract class StatUpgradeSO : UpgradeSO
{
    public float health;
    public float movement;
    public float rotation;
    public float xp;
    public abstract void ApplyUpgrade(StatsController statsController);
}

public abstract class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public abstract void ApplyUpgrade(ItemBase target);
}
