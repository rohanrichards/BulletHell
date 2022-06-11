using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial class DamagerRaycastSystem : SystemBase
{
    private EntityQuery entityQuery;
    private BeginInitializationEntityCommandBufferSystem ecbs;
    public float3 target;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadWrite<EntityDamageComponent>()
        );
        ecbs = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    public partial struct RaycastJob : IJobEntity
    {
        [ReadOnly] public CollisionWorld world;
        [ReadOnly] public float3 target;
        public EntityCommandBuffer ecb;

        public void Execute([EntityInQueryIndex] int index, Entity entity, in Translation position,  ref EntityDamageComponent damage)
        {
            if (math.length(position.Value - target) > damage.attackRange)
            {
                damage.attacking = false;
                damage.attackTime = 0;
                return;
            }
            RaycastInput input = new RaycastInput()
            {
                Start = position.Value,
                End = target,
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1 << 0, // enemies
                    CollidesWith = 1 << 2, // player
                    GroupIndex = 0
                }
            };
            RaycastHit hit;
            bool haveHit = world.CastRay(input, out hit);
            if (haveHit)
            {
                // set damage component
                damage.attacking = true;
            }
        }
    }
    protected override void OnUpdate()
    {
        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
        EntityCommandBuffer ecb = ecbs.CreateCommandBuffer();

        RaycastJob rcj = new RaycastJob
        {
            world = collisionWorld,
            target = target,
            ecb = ecb
        };
        
        JobHandle handle = rcj.Schedule(entityQuery);
        ecbs.AddJobHandleForProducer(handle);
    }
}
