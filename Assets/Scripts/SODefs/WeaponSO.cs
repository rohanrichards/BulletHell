using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "SO's/Configs/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    [Tooltip("Expressed as rounds per second")]
    public float rof = 1.0f;
    public float rofPercentBonus = 0f;
    public float rofFlatBonus = 0f;
    public float ROF
    {
        get { return rof + rofFlatBonus + rof * (rofPercentBonus / 100); }
    }
    public int projectileCount = 1;
    public int projectileCountBonus = 0;
    public float ProjectileCount
    {
        get { return projectileCount + projectileCountBonus; }
    }
    public float knockBackForce = 10;
    public float knockBackForcePercentBonus = 0;
    public float KnockBackForce
    {
        get { return knockBackForce + knockBackForce * (knockBackForcePercentBonus / 100); }
    }
}
