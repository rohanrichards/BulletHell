using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct MoveInDirectionJob : IJobEntity
{
    public float dt;
    public float3 direction;

    public void Execute(ref PhysicsVelocity velocity, ref Rotation rotation, ref Translation position, in EntityMovementSettings settings)
    {
        velocity.Linear += settings.moveSpeed * direction * dt;
    }
}