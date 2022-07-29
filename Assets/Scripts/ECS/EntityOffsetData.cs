using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//create position and rotation offsets to be used when making a child/parent attachment
public struct EntityOffsetData : IComponentData
{
    public Translation positionOffset;
    public Rotation rotationOffset;
}