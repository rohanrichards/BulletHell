using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    BeamLaser,
    DiscGun,
    Discharger,
    Laser,
    Launcher,
    Shotgun
};

public enum StatType {
    DamageIncrease,
    HealthIncrease,
    MovementIncrease,
    RoFIncrease,
    RotateIncrease,
    XPIncrease
};

[System.Serializable]
public class WeaponEntry
{
    public WeaponType label;
    public int level;
};

[System.Serializable]
public class StatEntry
{
    public StatType label;
    public int level;
};

[CreateAssetMenu(fileName = "Items_", menuName = "BulletHellTesting/ItemFixture", order = 1)]
public class BaseItemFixtureSO : ScriptableObject
{
    public int startingXP;

    [SerializeField]
    public WeaponEntry[] weapons;

    [SerializeField]
    public StatEntry[] stats;
}
