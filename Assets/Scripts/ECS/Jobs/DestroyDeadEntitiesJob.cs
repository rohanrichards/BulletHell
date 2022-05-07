using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using static EntityMessagingController;

[BurstCompile]
partial struct DestroyExpiredEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public void Execute(Entity entity, in LifespanComponent lifespan, in LocalToWorld local, in EntityDataComponent data)
    {
        if (lifespan.Value <= 0)
        {
            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { position = local.Position, rotation = local.Rotation, type = MessageTypes.Death });
            ecb.AddComponent(message, data);
            ecb.DestroyEntity(entity);
        }
    }
}

partial struct DestroyDeadEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public void Execute([EntityInQueryIndex] int index, Entity entity, in EntityHealthComponent health, in LocalToWorld local)
    {
        if (health.CurrentHealth <= 0)
        {
            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { position = local.Position, rotation = local.Rotation, type = MessageTypes.Death });
            ecb.DestroyEntity(entity);
        }
    }
}
