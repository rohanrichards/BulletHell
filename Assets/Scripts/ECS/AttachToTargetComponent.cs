using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntityMessagingController;

public struct AttachToTargetComponent : IComponentData
{
    public Entity target;
}