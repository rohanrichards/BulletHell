using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct RotateToDirectionJob : IJobEntity
{
    public float dt;
    public float3 direction;
    public float turnSpeed;

    public void Execute(ref Rotation rotation)
    {

        quaternion targetRotation = quaternion.LookRotationSafe(math.forward(), direction);
        rotation.Value = math.slerp(rotation.Value, targetRotation, turnSpeed);
    }
}
