using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using Unity.Collections;

public partial class EnemyRotatorSystem : SystemBase
{
    public float turnSpeed;
    private EntityQuery playerEntityQuery;
    private EntityQuery rotatingEntityQuery;

    protected override void OnStartRunning()
    {
        rotatingEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<RotateToTargetTag>()
        );

        playerEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
    }

    protected override void OnUpdate()
    {
        var location = playerEntityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        if(location.Length > 0)
        {
            RotateToTargetJob job = new RotateToTargetJob { dt = Time.DeltaTime, facingTarget = location[0].Value, turnSpeed = turnSpeed };
            job.Schedule(rotatingEntityQuery);
        }
    }
}
