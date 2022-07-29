using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public partial class DelayedDestroySystem : SystemBase
{
    private EntityQuery entityQuery;
    EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<DisableParticlesTag>()
        );
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities
            .WithAll<DelayedDestroyComponent>()
            .WithoutBurst()
            .ForEach((ref Entity entity, ref DelayedDestroyComponent delay) =>
            {
                if(delay.Value > 0)
                {
                    ecb.SetComponent(entity, new DelayedDestroyComponent { Value = delay.Value - Time.DeltaTime});
                }else
                {
                    ecb.DestroyEntity(entity);
                }
            }).Run();

    }
}