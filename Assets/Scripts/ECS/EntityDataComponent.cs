using System;
using Unity.Entities;

public enum EntityTypes
{
    // Death Types
    ExplodesOnDeath = 100,
    SplattersOnDeath = 101,
    DoesNothingOnDeath = 102,
    EndsGameOnDeath = 103,
    // Pickup Types
    XP = 201,
    Health = 202,
    Chest = 203,
}

public struct EntityDataComponent : IComponentData
{
    public EntityTypes Type;
    public float Size;
    public float Damage;
    public float Force;
    public float XP;
    public float Health;
}