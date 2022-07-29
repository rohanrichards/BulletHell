using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct AdjustTargetAtDistanceJob : IJobEntity
{
    public float dt;
    public float3 playerPosition;

    public void Execute(ref Rotation rotation, ref EntityTargetSettings targetSettings, in EntityMovementSettings moveSettings, in LocalToWorld localToWorld)
    {
        float distance = math.distance(localToWorld.Position, playerPosition);
        if(distance > 40)
        {
            //reassign target
            targetSettings.targetMovementDirection = playerPosition - localToWorld.Position;
        }
    }
}
