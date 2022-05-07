using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BulletMoverSystem))]
public partial class EnemyHealthSystem : SystemBase
{
    private EntityQueryDesc enemyQueryDesc;
    EndSimulationEntityCommandBufferSystem ecbs;
    private EntityQuery expiredQuery;
    private EntityQuery deadQuery;

    protected override void OnStartRunning()
    {
        expiredQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<LifespanComponent>()
        );

        deadQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<EntityHealthComponent>()
        );

        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        DestroyDeadEntitiesJob deadJob = new DestroyDeadEntitiesJob { ecb = ecbs.CreateCommandBuffer() };
        JobHandle jobHandle = deadJob.Schedule(deadQuery);
        jobHandle.Complete();

        DestroyExpiredEntitiesJob exJob = new DestroyExpiredEntitiesJob { ecb = ecbs.CreateCommandBuffer() };
        JobHandle exJobHandle = exJob.Schedule(expiredQuery);
        exJobHandle.Complete();
    }

}