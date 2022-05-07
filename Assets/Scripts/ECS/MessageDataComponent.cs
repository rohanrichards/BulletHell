using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MessageDataComponent : IComponentData
{
    public float3 position;
    public Quaternion rotation;
    public int type;
}