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
    public struct LightData {
        public int index;
        public float3 position;
        public float radius;
        public float3 rgb;
        public float intensity;
    }

    [BurstCompile]
    partial struct UpdateLightsJob : IJobEntity
    {
        [WriteOnly]
        public NativeList<LightData> lightPositions;
        public void Execute(Entity entity, in Translation position, in LightDataComponent lightData)
        {
            int index = entity.Index;
            lightPositions.Add(new LightData { index = index, position = position.Value, radius = lightData.radius, intensity = lightData.intensity, rgb = lightData.color });
        }
    }

    private EntityQuery lightEntityQuery;
    private NativeList<LightData> lightPositions;
    private LightsController lightsController;
    protected override void OnStartRunning()
    {
        lightsController = GameObject.Find("GameplayScripts").GetComponent<LightsController>();
        lightEntityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<LightDataComponent>()
        );
    }

    protected override void OnUpdate()
    {
        lightPositions = new NativeList<LightData>(Allocator.TempJob);
        UpdateLightsJob lightsJob = new UpdateLightsJob { };
        lightsJob.lightPositions = lightPositions;
        JobHandle handle = lightsJob.Schedule(lightEntityQuery);
        handle.Complete();
        lightsController.setLights(lightPositions);
        lightPositions.Dispose();
    }
}
