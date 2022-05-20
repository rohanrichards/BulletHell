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
        [ReadOnly] public ComponentDataFromEntity<ShootableTag> enemyGroup;
        [ReadOnly] public ComponentDataFromEntity<BulletConfigComponent> configGroup;
        public ComponentDataFromEntity<LifespanComponent> lifespanGroup;
        public ComponentDataFromEntity<EntityHealthComponent> healthGroup;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            Entity bullet;
            Entity shootable;

            if (bulletGroup.HasComponent(entityA) && enemyGroup.HasComponent(entityB)) 
            {
                bullet = entityA;
                shootable = entityB;
            } else if (bulletGroup.HasComponent(entityB) && enemyGroup.HasComponent(entityA))
            {
                bullet = entityB;
                shootable = entityA;
            } else
            {
                return;
            }

            LifespanComponent lifespan = lifespanGroup[bullet];
            if(lifespan.Value > 0)
            {
                // bulllet hit a shootable so stop it
                lifespan.Value = 0;
                lifespanGroup[bullet] = lifespan;

                //  if the shootable has health reduce it
                if (healthGroup.HasComponent(shootable))
                {
                    EntityHealthComponent health = healthGroup[shootable];
                    health.CurrentHealth -= configGroup[bullet].Damage;
                    healthGroup[shootable] = health;

                }
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
            enemyGroup = GetComponentDataFromEntity<ShootableTag>(true),
            configGroup = GetComponentDataFromEntity<BulletConfigComponent>(true),
            lifespanGroup = GetComponentDataFromEntity<LifespanComponent>(),
            healthGroup = GetComponentDataFromEntity<EntityHealthComponent>()
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);
    }
}
