using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct RotateToTargetJob : IJobEntity
{
    public float dt;
    public float3 facingTarget;
    public float turnSpeed;

    public void Execute(ref Rotation rotation, ref Translation position)
    {
        float3 heading = facingTarget - position.Value;
        quaternion targetRotation = quaternion.LookRotationSafe(math.forward(), heading);
        rotation.Value = math.slerp(rotation.Value, targetRotation, turnSpeed);
    }
}
