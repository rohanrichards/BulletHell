using System;
using Unity.Entities;
using Unity.Mathematics;

public struct EntityMovementSettings : IComponentData
{
    public float moveSpeed;
    public float3 targetMovementDirection;
    public float rotationSpeed;
}