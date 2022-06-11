using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static EntityMessagingController;

public enum MessageTypes
{
    Death = 100,
    Pickup = 200,
    Attack = 300
}
public struct MessageDataComponent : IComponentData
{
    public float3 position;
    public Quaternion rotation;
    public MessageTypes type;
}