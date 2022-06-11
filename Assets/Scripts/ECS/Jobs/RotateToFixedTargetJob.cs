using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
partial struct RotateToFixedTargetJob : IJobEntity
{
    public float dt;

    public void Execute(ref Rotation rotation, in EntityTargetSettings targetSettings, in EntityMovementSettings moveSettings)
    {
        quaternion targetRotation = quaternion.LookRotationSafe(math.forward(), targetSettings.targetMovementDirection);
        rotation.Value = math.slerp(rotation.Value, targetRotation, 1f);
    }
}
