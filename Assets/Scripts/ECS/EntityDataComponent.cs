using System;
using Unity.Entities;

public enum EntityDeathTypes
{
    // Death Types
    ExplodesOnDeath = 100,
    SplattersOnDeath = 101,
    DoesNothingOnDeath = 102,
    EndsGameOnDeath = 103
}

public struct EntityDataComponent : IComponentData
{
    public EntityDeathTypes Type;
    public float Size;
    public float Damage;
    public float Force;
    public float XP;
    public float Health;
    public bool Chest;
}