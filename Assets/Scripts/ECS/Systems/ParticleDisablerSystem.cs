using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

[UpdateAfter(typeof(EntityExpirationSystem))]
[UpdateAfter(typeof(TransformSystemGroup))]


public partial class ParticleDisablerSystem : SystemBase
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
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities
            .WithAll<ParticleSystem, DisableParticlesTag>()
            .WithoutBurst()
            .ForEach((ref Entity entity, in ParticleSystem particles) =>
            {
                EntityManager.GetComponentObject<ParticleSystem>(entity);
                ecb.AddComponent(entity, new DelayedDestroyComponent { Value = 5 });
                particles.Stop();
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

    }
}