using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct EntityXPComponent : IComponentData
{
    public float CurrentXP;
}
