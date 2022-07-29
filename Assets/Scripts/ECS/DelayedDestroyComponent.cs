using System;
using Unity.Entities;

public struct DelayedDestroyComponent : IComponentData
{
    public float Value;
}