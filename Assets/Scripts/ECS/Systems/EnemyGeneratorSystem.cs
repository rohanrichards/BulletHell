using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics.Systems;

public partial class EnemyGeneratorSystem : SystemBase
{
    private EntityQuery playerQuery;
    protected override void OnStartRunning()
    {
        playerQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadOnly<PlayerTag>()
        );
    }

    public NativeList<Translation> GetClusterLocations(int number, int radius, float2 biasAngleArc)
    {
        NativeArray<Translation> playerLocations = playerQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        NativeList<Translation> spawnLocations = new NativeList<Translation>(number, Allocator.Temp);
        var playerLocation = playerLocations[0];

        for (int i = 0; i < number; i++)
        {
            float3 clusterLocation = playerLocation.Value + RandomPointOnCircleEdgeBiased(radius, biasAngleArc.x, biasAngleArc.y);
            int attempts = 0;
            int maxAttempts = 5;
            while(attempts < maxAttempts)
            {
                if(!CheckForHitsAtLocationInRange(clusterLocation, 5))
                {
                    spawnLocations.Add(new Translation() { Value= clusterLocation });
                    break;
                }
                else
                {
                    attempts++;
                }
            }
        }

        return spawnLocations;
    }

    private bool CheckForHitsAtLocationInRange(float3 location, int range)
    {
        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
        NativeList<int> hitsIndices = new NativeList<int>(Allocator.Temp);
        CollisionFilter filter = new CollisionFilter
        {
            BelongsTo = 1 << 0,
            CollidesWith = 1 << 0,
            GroupIndex = 0
        };
        Aabb aabb = new Aabb
        {
            Min = location + new float3(-range, -range, -1),
            Max = location + new float3(range, range, 1)
        };
        OverlapAabbInput overlapAabbInput = new OverlapAabbInput
        {
            Aabb = aabb,
            Filter = filter,
        };

        collisionWorld.OverlapAabb(overlapAabbInput, ref hitsIndices);

        if(hitsIndices.Length > 0)
        {
            return true;
        }else
        {
            return false;
        }
    }

    private float3 RandomPointOnCircleEdgeBiased(float radius, float biasAngleDegree, float biasArcSizeDegrees)
    {
        float radians = (biasAngleDegree + (UnityEngine.Random.value - 0.5f) * biasArcSizeDegrees) * 0.01745329251f; // convert degrees to radians
        float2 point = new float2(math.cos(radians), math.sin(radians)) * radius;
        return new float3(point.x, point.y, 0);
    }

    private float3 RandomPointOnCircleEdge(float radius)
    {
        float2 point = UnityEngine.Random.insideUnitCircle.normalized * radius;
        return new float3(point.x, point.y, 0);
    }

    private float3 RandomPointInsideCircle(float radius)
    {
        float2 point = UnityEngine.Random.insideUnitCircle * radius;
        return new float3(point.x, point.y, 0);
    }

    protected override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }
}