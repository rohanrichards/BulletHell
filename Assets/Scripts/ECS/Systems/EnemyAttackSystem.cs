using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public partial class EnemyAttackSystem : SystemBase
{
    public float3 target;
    private EntityQuery entityQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;
    private EntityQuery playerQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadWrite<EntityDamageComponent>()
        );

        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadWrite<EntityHealthComponent>()
        );
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        NativeArray<EntityHealthComponent> health = playerQuery.ToComponentDataArray<EntityHealthComponent>(Allocator.Temp);
        NativeArray<Entity> target = playerQuery.ToEntityArray(Allocator.Temp);
        if(health.Length > 0 && target.Length > 0)
        {
            AttackTargetJob job = new AttackTargetJob { dt = Time.DeltaTime, targetHealth = health[0], ecb = ecbs.CreateCommandBuffer(), target = target[0] };
            job.Schedule(entityQuery);
        }
    }
}