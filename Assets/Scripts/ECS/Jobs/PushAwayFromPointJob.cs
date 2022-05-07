using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics.Extensions;

[BurstCompile]
partial struct PushAwayFromPointJob : IJobEntity
{
    public float dt;
    public float force;
    public float3 center;
    public float radius;
    [ReadOnly] public PhysicsWorld world;

    public void Execute(ref PhysicsVelocity velocity, ref Translation position)
    {
            float3 direction = position.Value - center;
            float falloff = radius / math.length(direction);
            velocity.Linear += math.normalize(direction) * force * dt * falloff;
    }
}