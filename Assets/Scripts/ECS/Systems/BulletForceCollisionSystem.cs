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
public partial class BulletForceCollisionSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    struct DamageOnCollisionJob : ICollisionEventsJob
    {
        public PhysicsWorld physicsWorld;
        public float deltaTime;
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletGroup;
        [ReadOnly] public ComponentDataFromEntity<ShootableTag> enemyGroup;
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> velGroup;
        [ReadOnly] public ComponentDataFromEntity<Rotation> rotationGroup;
        [ReadOnly] public ComponentDataFromEntity<BulletConfigComponent> configGroup;
        [ReadOnly] public ComponentDataFromEntity<KnockableTag> knockGroup;
        [ReadOnly] public ComponentDataFromEntity<ForceDamageTag> forceGroup;
        public ComponentDataFromEntity<LifespanComponent> lifespanGroup;
        public ComponentDataFromEntity<EntityHealthComponent> healthGroup;
        public void Execute(CollisionEvent triggerEvent)
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
            if (!forceGroup.HasComponent(bullet)) {
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

                        lifespanGroup[bullet] = handleLifespan(bullet);
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
                    lifespanGroup[bullet] = handleLifespan(bullet);
                }

                // check if the target can be knocked back
                if (knockGroup.HasComponent(shootable) && shouldKnock)
                {
                    float force = configGroup[bullet].Knockback;
                    float3 vel = math.normalize(velGroup[bullet].Linear);
                    if(float.IsNaN(vel.x) || float.IsNaN(vel.y))
                    {
                        float3 direction = new float3 { x = 0, y = 1, z = 0 };
                        quaternion rotation = rotationGroup[bullet].Value;
                        vel = math.mul(rotation, direction);
                    }
                    PhysicsWorldExtensions.ApplyLinearImpulse(physicsWorld, shootableBodyIndex, force * vel * deltaTime);
                }
            }
        }

        private LifespanComponent handleLifespan(Entity entity)
        {
            BulletConfigComponent bulletConfig = configGroup[entity];
            LifespanComponent lifespan = lifespanGroup[entity];
            if (bulletConfig.DOT)
            {
                return lifespan;
            }else
            {
                lifespan.Value = 0;
                return lifespan;
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
        Dependency = new DamageOnCollisionJob {
            physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld,
            deltaTime = Time.DeltaTime,
            bulletGroup = GetComponentDataFromEntity<BulletTag>(true),
            enemyGroup = GetComponentDataFromEntity<ShootableTag>(true),
            knockGroup = GetComponentDataFromEntity<KnockableTag>(true),
            forceGroup = GetComponentDataFromEntity<ForceDamageTag>(true),
            configGroup = GetComponentDataFromEntity<BulletConfigComponent>(true),
            velGroup = GetComponentDataFromEntity<PhysicsVelocity>(true),
            rotationGroup = GetComponentDataFromEntity<Rotation>(true),
            lifespanGroup = GetComponentDataFromEntity<LifespanComponent>(),
            healthGroup = GetComponentDataFromEntity<EntityHealthComponent>()
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);

        this.CompleteDependency();
    }
}
