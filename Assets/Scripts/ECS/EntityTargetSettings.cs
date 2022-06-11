using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EntityTargetSettings : IComponentData
{
    public float3 targetMovementDirection;
}