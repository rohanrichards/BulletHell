using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class FirePatternSO : ScriptableObject
{
    public string patternName;
    public abstract List<Entity> Fire(WeaponSO weaponConfig, Entity bulletPrefab);
}