using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public partial class BulletMoverSystem : SystemBase
{
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<MoveForwardTag>(),
            ComponentType.ReadWrite<LifespanComponent>()
        );
    }

    protected override void OnUpdate()
    {
        MoveForwardJob moveJob = new MoveForwardJob { dt = Time.DeltaTime };
        moveJob.Schedule(entityQuery);

        LifespanReducerJob lifeJob = new LifespanReducerJob { dt = Time.DeltaTime };
        lifeJob.Schedule(entityQuery);
    }
}