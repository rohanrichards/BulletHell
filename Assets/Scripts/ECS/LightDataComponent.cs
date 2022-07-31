using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct LightDataComponent : IComponentData
{
    public float3 color;
    public float radius;
    public float intensity;
}
