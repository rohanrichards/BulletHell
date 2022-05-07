using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct DestroyDeadEntitiesJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public void Execute(in Entity entity)
    {
        ecb.DestroyEntity(entity);
    }
}
