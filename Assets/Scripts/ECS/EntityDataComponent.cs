using System;
using Unity.Entities;

public enum EntityTypes
{
    ExplodesOnDeath = 100,
    SplattersOnDeath = 200,
    DoesNothingOnDeath = 300,
}

public struct EntityDataComponent : IComponentData
{
    public EntityTypes Type;
    public float Size;
    public float Damage;
    public float Force;
}