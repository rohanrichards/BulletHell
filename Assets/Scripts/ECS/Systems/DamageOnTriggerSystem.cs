using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class DamageOnTriggerSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    struct DamageOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemyGroup;
        [ReadOnly] public ComponentDataFromEntity<BulletConfigComponent> configGroup;
        public ComponentDataFromEntity<LifespanComponent> lifespanGroup;
        public ComponentDataFromEntity<EntityHealthComponent> healthGroup;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            Entity bullet;
            Entity enemy;

            if (bulletGroup.HasComponent(entityA) && enemyGroup.HasComponent(entityB)) 
            {
                bullet = entityA;
                enemy = entityB;
            } else
            {
                bullet = entityB;
                enemy = entityA;
            }

            LifespanComponent lifespan = lifespanGroup[bullet];
            if(lifespan.Value > 0)
            {
                EntityHealthComponent health = healthGroup[enemy];
                health.CurrentHealth -= configGroup[bullet].Damage;
                healthGroup[enemy] = health;

                lifespan.Value = 0;
                lifespanGroup[bullet] = lifespan;
            }
        }
    }

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        Dependency = new DamageOnTriggerJob{
            bulletGroup = GetComponentDataFromEntity<BulletTag>(true),
            enemyGroup = GetComponentDataFromEntity<EnemyTag>(true),
            configGroup = GetComponentDataFromEntity<BulletConfigComponent>(true),
            lifespanGroup = GetComponentDataFromEntity<LifespanComponent>(),
            healthGroup = GetComponentDataFromEntity<EntityHealthComponent>()
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);
    }
}
