using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using static EntityMessagingController;
using Unity.Collections;

[BurstCompile]
partial struct DestroyExpiredEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public ComponentDataFromEntity<Translation> translationsGroup;
    public ComponentDataFromEntity<Rotation> rotationsGroup;
    public ComponentDataFromEntity<LocalToWorld> localGroup;

    public void Execute(Entity entity, ref LifespanComponent lifespan, in EntityDataComponent data, ref DynamicBuffer<LinkedEntityGroup> linkedEntityGroup)
    {
        if (lifespan.Value <= 0)
        {

            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { position = localGroup[entity].Position, rotation = localGroup[entity].Rotation, type = MessageTypes.Death });
            ecb.AddComponent(message, data);
            ecb.DestroyEntity(entity);
        }
    }
}

[BurstCompile]
partial struct DeparentDeadEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    [ReadOnly] public ComponentDataFromEntity<Translation> translationsGroup;
    [ReadOnly] public ComponentDataFromEntity<Rotation> rotationsGroup;
    [ReadOnly] public ComponentDataFromEntity<LocalToWorld> localGroup;

    public void Execute(Entity entity, ref LifespanComponent lifespan, in DeparentTag deparent, ref DynamicBuffer<LinkedEntityGroup> linkedEntityGroup)
    {
        if (lifespan.Value <= 0)
        {
            if (!linkedEntityGroup.IsEmpty)
            {
                NativeArray<LinkedEntityGroup> children = linkedEntityGroup.ToNativeArray(Unity.Collections.Allocator.Temp);
                for (int i = 0; i < children.Length; i++)
                {
                    Entity child = children[i].Value;
                    Translation childTranslation = new Translation { Value = localGroup[child].Position };
                    Rotation childRotation = rotationsGroup[child];

                    ecb.SetComponent(child, childTranslation);
                    ecb.SetComponent(child, childRotation);

                    ecb.AddComponent<DisableParticlesTag>(child);
                }
            }
            linkedEntityGroup.RemoveRange(0, linkedEntityGroup.Length);
        }
    }
}

[BurstCompile]
partial struct DestroyDeadEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public void Execute(Entity entity, in EntityHealthComponent health, in LocalToWorld local, in EntityDataComponent data)
    {
        if (health.CurrentHealth <= 0)
        {
            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { position = local.Position, rotation = local.Rotation, type = MessageTypes.Death });
            ecb.AddComponent(message, data);
            ecb.DestroyEntity(entity);
        }
    }
}
