using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
partial struct LifespanReducerJob : IJobEntity
{
    public float dt;
    public EntityCommandBuffer ecb;

    public void Execute([EntityInQueryIndex] int index, Entity entity, ref LifespanComponent lifespan, in LocalToWorld local, in EntityTypeComponent type)
    {
        lifespan.Value -= dt;
        if(lifespan.Value <= 0)
        {
            Entity message = ecb.CreateEntity();
            ecb.AddComponent(message, new MessageDataComponent { position = local.Position, rotation = local.Rotation, type = 100 });
            ecb.AddComponent(message, type);
            ecb.DestroyEntity(entity);
        }
    }
}