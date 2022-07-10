using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BuildPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class BulletCollisionSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    struct DamageOnTriggerJob : ITriggerEventsJob
    {
        public PhysicsWorld physicsWorld;
        public float deltaTime;
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletGroup;
        [ReadOnly] public ComponentDataFromEntity<ShootableTag> enemyGroup;
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> velGroup;
        [ReadOnly] public ComponentDataFromEntity<BulletConfigComponent> configGroup;
        [ReadOnly] public ComponentDataFromEntity<KnockableTag> knockGroup;
        public ComponentDataFromEntity<LifespanComponent> lifespanGroup;
        public ComponentDataFromEntity<EntityHealthComponent> healthGroup;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity bullet;
            Entity shootable;
            int shootableBodyIndex;
            bool shouldKnock = true;

            if (bulletGroup.HasComponent(triggerEvent.EntityA) && enemyGroup.HasComponent(triggerEvent.EntityB)) 
            {
                bullet = triggerEvent.EntityA;
                shootable = triggerEvent.EntityB;
                shootableBodyIndex = physicsWorld.GetRigidBodyIndex(shootable);
            } else if (bulletGroup.HasComponent(triggerEvent.EntityB) && enemyGroup.HasComponent(triggerEvent.EntityA))
            {
                bullet = triggerEvent.EntityB;
                shootable = triggerEvent.EntityA;
                shootableBodyIndex = physicsWorld.GetRigidBodyIndex(shootable);
            } else
            {
                return;
            }

            LifespanComponent lifespan = lifespanGroup[bullet];
            if(lifespan.Value > 0)
            {
                //  if the shootable has health reduce it
                if (healthGroup.HasComponent(shootable))
                {
                    EntityHealthComponent health = healthGroup[shootable];
                    if(health.CurrentHealth > 0)
                    {
                        health.CurrentHealth -= configGroup[bullet].Damage;
                        healthGroup[shootable] = health;

                        // bullet reduced health so destroy the bullet
                        lifespan.Value = 0;
                        lifespanGroup[bullet] = lifespan;
                    }

                    // if their health is now below zero we dont want to knock
                    if(health.CurrentHealth <= 0)
                    {
                        shouldKnock = false;
                    }
                }else
                {
                    // the bullet hit a shootable but it didn't have health
                    // we just stop the bullet in this case
                    lifespan.Value = 0;
                    lifespanGroup[bullet] = lifespan;
                }

                // check if the target can be knocked back
                if (knockGroup.HasComponent(shootable) && shouldKnock)
                {
                    float force = configGroup[bullet].Knockback;
                    float3 vel = math.normalize(velGroup[bullet].Linear);
                    PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, shootableBodyIndex, force * vel * deltaTime);
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
            physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld,
            deltaTime = Time.DeltaTime,
            bulletGroup = GetComponentDataFromEntity<BulletTag>(true),
            enemyGroup = GetComponentDataFromEntity<ShootableTag>(true),
            knockGroup = GetComponentDataFromEntity<KnockableTag>(true),
            configGroup = GetComponentDataFromEntity<BulletConfigComponent>(true),
            velGroup = GetComponentDataFromEntity<PhysicsVelocity>(true),
            lifespanGroup = GetComponentDataFromEntity<LifespanComponent>(),
            healthGroup = GetComponentDataFromEntity<EntityHealthComponent>()
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);
    }
}
