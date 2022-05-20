using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public partial class LifespanReducerSystem : SystemBase
{
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<LifespanComponent>()
        );
    }

    protected override void OnUpdate()
    {
        LifespanReducerJob lifeJob = new LifespanReducerJob { dt = Time.DeltaTime };
        lifeJob.Schedule(entityQuery);
    }
}