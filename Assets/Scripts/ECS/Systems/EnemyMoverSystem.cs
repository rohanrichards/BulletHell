using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public partial class EnemyMoverSystem : SystemBase
{
    public float3 target;
    private EntityQuery movingEntityQuery;
    private EntityQuery playerEntityQuery;
    protected override void OnStartRunning()
    {
        movingEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<EntityMovementSettings>(),
            ComponentType.ReadOnly<MoveTowardTargetTag>()
        );

        playerEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadWrite<Translation>()
        );
    }

    protected override void OnUpdate()
    {
        var location = playerEntityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        if (location.Length > 0)
        {
            MoveTowardTarget job = new MoveTowardTarget { dt = Time.DeltaTime, target = location[0].Value };
            job.Schedule(movingEntityQuery);
        }
    }
}