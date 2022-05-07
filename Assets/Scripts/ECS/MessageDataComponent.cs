using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntityMessagingController;

public struct MessageDataComponent : IComponentData
{
    public float3 position;
    public Quaternion rotation;
    public MessageTypes type;
}