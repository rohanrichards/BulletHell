using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
partial struct MoveTowardTarget : IJobEntity
{
    public float dt;
    public float3 target;

    public void Execute(ref PhysicsVelocity velocity, ref Translation position, in EntityMovementSettings settings)
    {
        float3 diff = target - position.Value;
        float dist = math.length(diff);
        velocity.Linear += settings.moveSpeed * (diff / dist) * dt;
    }
}