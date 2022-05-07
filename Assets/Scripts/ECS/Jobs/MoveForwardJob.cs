using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
partial struct MoveForwardJob : IJobEntity
{
    public float dt;

    public void Execute(ref PhysicsVelocity velocity, in EntityMovementSettings settings, in LocalToWorld local)
    {
        velocity.Linear += settings.moveSpeed * local.Up * dt;
    }
}