using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BulletConfigComponent : IComponentData
{
    public float Damage;
    public float Knockback;
    public float Size;
    public bool DOT;
}
