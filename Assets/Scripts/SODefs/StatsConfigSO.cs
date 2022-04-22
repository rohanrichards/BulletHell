using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats Config", menuName = "SO's/Configs/Stats", order = 1)]
public class StatsConfigSO : ScriptableObject
{
    public int currentLevel;
    public int currentXP;
    public float XPPercentBonus;
    public int nextLevelXP;
    public int previousLevelXP;
    public int baseHealth;
    public int currentHealth;
    public float healthPercentBonus;
    public int MaxHealth
    {
        get { return Mathf.RoundToInt(baseHealth + baseHealth * (healthPercentBonus / 100)); }
    }
    public float moveSpeed;
    public float moveSpeedPercentBonus;
    public float MoveSpeed
    {
        get { return moveSpeed + moveSpeed * (moveSpeedPercentBonus / 100); }
    }
    public float rotateSpeed;
    public float rotateSpeedPercentBonus;
    public float RotateSpeed
    {
        get { return rotateSpeed + rotateSpeed * (rotateSpeedPercentBonus / 100); }
    }
}
