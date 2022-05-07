using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using static EntityMessagingController;

[BurstCompile]
partial struct LifespanReducerJob : IJobEntity
{
    public float dt;
    public EntityCommandBuffer ecb;

    public void Execute(ref LifespanComponent lifespan)
    {
        lifespan.Value -= dt;
    }
}