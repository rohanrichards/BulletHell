using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "SO's/Configs/Bullet", order = 2)]
public class BulletSO : ScriptableObject
{
    public float Damage = 1f;
    public float Speed = 50f;
    public float Lifespan = 0.25f;
    public float AOE = 1f;
}
