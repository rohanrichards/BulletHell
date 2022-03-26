using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Stat", menuName = "SO's/Configs/Global", order = 1)]
public class GlobalStatsConfigSO : ScriptableObject
{
    [Tooltip("Expressed as rounds per second")]
    public float rofPercentBonus = 0f;
    public int projectileCountBonus = 0;
    public int projectilePierceBonus = 0;
    public float damagePercentBonus = 0f;
    public float areaPercentBonus = 0f;
    public float knockbackPercentBonus = 0f;
}
