using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

public partial class AttachedMoverSystem : SystemBase
{
    private EntityQuery entityQuery;
    protected override void OnStartRunning()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadWrite<AttachToTargetComponent>()
        );
    }

    [BurstCompile]
    partial struct MoveAttachedEntityJob: IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> translationsGroup;

        public void Execute(ref Translation position, in AttachToTargetComponent targetData)
        {
            position = translationsGroup[targetData.target];
        }
    }

    protected override void OnUpdate()
    {
        MoveAttachedEntityJob moveJob = new MoveAttachedEntityJob { translationsGroup = GetComponentDataFromEntity<Translation>(true) };
        moveJob.Schedule(entityQuery);
    }
}