using System;
using Unity.Entities;

public struct EntityHealthComponent : IComponentData
{
    public float MaxHealth;
    public float CurrentHealth;
}