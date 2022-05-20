using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BuildPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class ExplosionSystem : SystemBase
{
    public NativeArray<Translation> locations;
    private EntityQuery entityQuery;
    private EntityQuery playerQuery;
    private EndSimulationEntityCommandBufferSystem ecbs;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<EntityDataComponent>(),
            ComponentType.ReadWrite<ExplodeAndDeleteTag>()
        );
        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        NativeArray<Translation> locations = entityQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<Translation> playerLocations = playerQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeArray<EntityDataComponent> datas = entityQuery.ToComponentDataArray<EntityDataComponent>(Allocator.Temp);

        for (int i = 0; i < locations.Length; i++)
        {
            var location = locations[i];
            var data = datas[i];
            var playerLocation = playerLocations[0];
            var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
            var world = physicsWorldSystem.PhysicsWorld;
            NativeList<int> hitsIndices = new NativeList<int>(Allocator.Temp);
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = 1 << 1,
                CollidesWith = 1 << 0,
                GroupIndex = 0
            };
            Aabb aabb = new Aabb
            {
                Min = location.Value + new float3(-data.Size, -data.Size, -1),
                Max = location.Value + new float3(data.Size, data.Size, 1)
            };
            OverlapAabbInput overlapAabbInput = new OverlapAabbInput
            {
                Aabb = aabb,
                Filter = filter,
            };

            collisionWorld.OverlapAabb(overlapAabbInput, ref hitsIndices);
            var bodies = collisionWorld.Bodies;
            var sqrRadius = data.Size * data.Size;

            for (int z = 0; z < hitsIndices.Length; z++)
            {
                int rigidBodyIndex = hitsIndices[z];
                RigidBody rigidBody = bodies[rigidBodyIndex];
                Entity entity = rigidBody.Entity;
                Translation position = GetComponent<Translation>(entity);
                float3 distanceOfPlayer = playerLocation.Value - location.Value;
                float3 directionOfPlayer = math.normalize(distanceOfPlayer);
                if (math.lengthsq(location.Value - position.Value) <= sqrRadius)
                {
                    //location.Value -= directionOfPlayer;
                    float3 direction = position.Value - location.Value;
                    float falloff = 1 - math.length(direction) / data.Size;
                    float3 force = math.normalize(direction) * data.Force * falloff * Time.DeltaTime;
                    PhysicsWorldExtensions.ApplyLinearImpulse(world, rigidBodyIndex, force);

                    EntityHealthComponent health = GetComponent<EntityHealthComponent>(entity);
                    health.CurrentHealth -= data.Damage * falloff;
                    EntityManager.SetComponentData(entity, health);
                }
            }
        }
        EntityManager.DestroyEntity(entityQuery);
    }
}