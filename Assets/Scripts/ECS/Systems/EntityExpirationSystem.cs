using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[UpdateBefore(typeof(ParticleDisablerSystem))]
public partial class EntityExpirationSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbs;
    private EntityQuery expiredQuery;
    private EntityQuery deadQuery;
    private EntityQuery deparentQuery;

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

        deparentQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<LifespanComponent>(),
            ComponentType.ReadOnly<DeparentTag>()
        );

        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        DeparentDeadEntitiesJob deparentJob = new DeparentDeadEntitiesJob
        {
            ecb = ecbs.CreateCommandBuffer(),
            translationsGroup = GetComponentDataFromEntity<Translation>(true),
            rotationsGroup = GetComponentDataFromEntity<Rotation>(true),
            localGroup = GetComponentDataFromEntity<LocalToWorld>(true)
        };
        JobHandle deparentJobHandle = deparentJob.Schedule(deparentQuery);
        deparentJobHandle.Complete();

        DestroyDeadEntitiesJob deadJob = new DestroyDeadEntitiesJob { ecb = ecbs.CreateCommandBuffer() };
        JobHandle jobHandle = deadJob.Schedule(deadQuery);
        jobHandle.Complete();

        DestroyExpiredEntitiesJob exJob = new DestroyExpiredEntitiesJob 
        { 
            ecb = ecbs.CreateCommandBuffer(), 
            translationsGroup = GetComponentDataFromEntity<Translation>(false),
            rotationsGroup = GetComponentDataFromEntity<Rotation>(false),
            localGroup = GetComponentDataFromEntity<LocalToWorld>(false)
        };
        JobHandle exJobHandle = exJob.Schedule(expiredQuery);
        exJobHandle.Complete();
    }

}