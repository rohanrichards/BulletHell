using System;
using Unity.Entities;

public enum EntityTypes
{
    ExplodesOnDeath = 100,
    SplayyersOnDeath = 200
}

public struct EntityDataComponent : IComponentData
{
    public EntityTypes Type;
    public float Size;
    public float Damage;
}