using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct EntityDamageComponent : IComponentData
{
    public float Damage;
    public bool attacking;
    public float attackTime;
    public float attackCooldown;
    public float attackRange;
}
