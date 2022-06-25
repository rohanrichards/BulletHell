using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class LightManagerSystem : SystemBase
{
    public struct lightInfo {
        public int index;
        public float3 position;
        public float radius;
        public float3 rgb;
    }

    [BurstCompile]
    partial struct UpdateLightsJob : IJobEntity
    {
        [WriteOnly]
        public NativeList<lightInfo> lightPositions;
        public void Execute(Entity entity, in Translation position)
        {
            Debug.Log("found entity");
            int index = entity.Index;
            lightPositions.Add(new lightInfo { index = index, position = position.Value, radius = 1.0f, rgb = {x = 1, y = 0, z = 1 } });
        }
    }

    private EntityQuery lightEntityQuery;
    private NativeList<lightInfo> lightPositions;
    private LightsController lightsController;
    protected override void OnStartRunning()
    {
        lightsController = GameObject.Find("GameplayScripts").GetComponent<LightsController>();
        lightEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<LightTag>()
        );
    }

    protected override void OnUpdate()
    {
        lightPositions = new NativeList<lightInfo>(Allocator.TempJob);
        UpdateLightsJob lightsJob = new UpdateLightsJob { };
        lightsJob.lightPositions = lightPositions;
        JobHandle handle = lightsJob.Schedule(lightEntityQuery);
        handle.Complete();

        Debug.Log(lightPositions.Length);
        lightsController.setLights(lightPositions);

        lightPositions.Dispose();
    }
}
