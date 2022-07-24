using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics.Systems;

public partial class ChargingEnemyMoverSystem : SystemBase
{
    private EntityQuery movingEntityQuery;
    protected override void OnStartRunning()
    {
        movingEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<EntityMovementSettings>(),
            ComponentType.ReadWrite<EntityTargetSettings>(),
            ComponentType.ReadOnly<MoveForwardTag>(),
            ComponentType.ReadOnly<RotateToFixedTargetTag>(),
            ComponentType.ReadOnly<LocalToWorld>()
        );
    }

    protected override void OnUpdate()
    {
        MoveForwardJob moveJob = new MoveForwardJob { dt = Time.DeltaTime };
        moveJob.ScheduleParallel(movingEntityQuery);

        AdjustTargetAtDistanceJob adjustJob = new AdjustTargetAtDistanceJob { dt = Time.DeltaTime, playerPosition = ECSPlayerController.getPlayerLocation().Position };
        adjustJob.ScheduleParallel(movingEntityQuery);

        RotateToFixedTargetJob rotateJob = new RotateToFixedTargetJob { dt = Time.DeltaTime };
        rotateJob.ScheduleParallel(movingEntityQuery);
    }
}